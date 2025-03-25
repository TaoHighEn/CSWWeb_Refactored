using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Data;
using CSWWeb.Lib.Model;
using CSWWeb.Lib.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSWWeb.Lib.Services
{
    public class BackgroundSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;
        private Timer _timer;
        private bool _isSyncEnabled = true;
        private int _sync_time = 15;
        private int _cache_expire_hour = 72;
        private readonly object _syncLock = new object();
        private readonly string _authkey;
        private readonly string _logkey;
        public BackgroundSyncService(
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            int.TryParse(configuration["SyncSettings:IntervalMinutes"], out _sync_time);
            int.TryParse(configuration["SyncSettings:ExpireHours"], out _cache_expire_hour);
            _authkey = configuration["MemoryCacheKey:AuthKey"];
            _logkey = configuration["MemoryCacheKey:LogKey"];
        }

        // 停止同步
        public void StopSync()
        {
            _isSyncEnabled = false;
            LogSyncActivity("Stopped", "手動停止同步");
        }

        // 啟動同步
        public void StartSync()
        {
            _isSyncEnabled = true;
            LogSyncActivity("Started", "手動啟動同步");
            SyncData();
        }

        // 手動觸發同步
        public void TriggerSync()
        {
            if (_isSyncEnabled)
            {
                SyncData();
            }
        }
        public bool Status()
        {
            return _isSyncEnabled;
        }

        // 記錄同步活動
        private void LogSyncActivity(string status, string description, string source = "系統")
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContextProvider = scope.ServiceProvider.GetRequiredService<DbContextProvider>();
                    var dbContext = dbContextProvider.GetDbContext();
                    var DB = dbContext.Database.IsSqlServer() ? "MSSQL" : "Sqlite";
                    //寫一筆同步資料進資料庫中
                    dbContext.Set<TbSysWsLog>().Add(new TbSysWsLog
                    {
                        WsAp = "System",
                        // 設定 LogMessage、預設 LogType 為 INFO，並記錄 LogStatus 與目前 UTC 時間
                        WsModule = "Sync",
                        LogMessage = "同步資料Log From " + DB,
                        LogType = "INFO",
                        LogStatus = "Received",
                        Datestamp = DateTime.UtcNow,
                    });
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Only if you need to access scoped services during initialization
            }
            // 在啟動時執行一次同步
            SyncData();

            // 設定每15分鐘執行一次的計時器
            TimeSpan interval = TimeSpan.FromSeconds(_sync_time);
            _timer = new Timer(SyncCallback, _isSyncEnabled, interval, interval);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _timer?.Change(Timeout.Infinite, 0);
        }

        /// <summary>
        /// timer callback
        /// </summary>
        /// <param name="state"></param>
        private void SyncCallback(object state)
        {
            if (_isSyncEnabled)
            {
                SyncData();
            }
        }

        // 同步資料
        private void SyncData()
        {
            if (!_isSyncEnabled)
                return;

            if (!Monitor.TryEnter(_syncLock, 0))
            {
                return;
            }

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContextProvider = scope.ServiceProvider.GetRequiredService<DbContextProvider>();
                var dbContext = dbContextProvider.GetDbContext();
                var (accounts, modules, privileges) = FetchDataFromDb(dbContext);
                var DB = dbContext.Database.IsSqlServer() ? "MSSQL" : "Sqlite";
                //更新Cache
                UpdateMemoryCache(accounts, modules, privileges, DB);
                //將memorycache的log存入DB
                LogProcessData(dbContext);
                //整理Log
                if (dbContext.Database.IsSqlServer())
                {
                    //todo 將account、module、priv同步到sqlite
                    BackupToSqlite(accounts, modules, privileges);
                    //todo 檢查有無loss的Log在Sqlite中存回mssql
                    CheckSqliteLog(dbContext);
                }
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// 將Sqlite紀錄的Log存回MSSQL
        /// </summary>
        /// <param name="dbContext"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckSqliteLog(DbContext dbContext)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                    if (context.TbSysWsLogs.Count() != 0)
                    {
                        List<TbSysWsLog> data = context.TbSysWsLogs.AsNoTracking().ToList();
                        data.ForEach(x => x.Seq = 0);
                        dbContext.Set<TbSysWsLog>().AddRange(data);
                        dbContext.SaveChangesAsync();

                        context.TbSysWsLogs.ExecuteDelete();
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 將Log寫入DB
        /// </summary>
        /// <param name="context"></param>
        public void LogProcessData(DbContext context)
        {
            try
            {
                CacheData cacheData = new CacheData();
                _memoryCache.TryGetValue(_logkey, out cacheData);
                if (cacheData != null)
                {
                    var logdata = cacheData.GetList<TbSysWsLog>();
                    context.Set<TbSysWsLog>().AddRange(logdata);
                    int i = context.SaveChanges();
                    _memoryCache.Remove(_logkey);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 從資料庫取得數據
        /// </summary>
        private (List<TbSysWsAccount>, List<TbSysWsModule>, List<TbSysWsPriv>) FetchDataFromDb(DbContext dbContext)
        {
            var accounts = dbContext.Set<TbSysWsAccount>().ToList();
            var privileges = dbContext.Set<TbSysWsPriv>().ToList();
            var modules = dbContext.Set<TbSysWsModule>().ToList();

            return (accounts, modules, privileges);
        }

        /// <summary>
        /// 更新 MemoryCache
        /// </summary>
        private void UpdateMemoryCache(List<TbSysWsAccount> accounts, List<TbSysWsModule> modules, List<TbSysWsPriv> privileges, string source)
        {

            var cacheData = new CacheData();
            cacheData.SetList(accounts);
            cacheData.SetList(modules);
            cacheData.SetList(privileges);
            _memoryCache.Set(_authkey, cacheData, TimeSpan.FromHours(_cache_expire_hour));
        }

        /// <summary>
        /// 備份至 SQLite
        /// </summary>
        private void BackupToSqlite(List<TbSysWsAccount> accounts, List<TbSysWsModule> modules, List<TbSysWsPriv> privileges)
        {
            using (var scope = _serviceProvider.CreateScope()) // 讓 BackgroundService 內部管理 Scoped
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                    Console.WriteLine($"[INFO] Using database: {dbContext.Database.GetDbConnection().ConnectionString}");
                    dbContext.Database.EnsureCreated();

                    dbContext.TbSysWsAccounts.RemoveRange(dbContext.TbSysWsAccounts);
                    dbContext.TbSysWsModules.RemoveRange(dbContext.TbSysWsModules);
                    dbContext.TbSysWsPrivs.RemoveRange(dbContext.TbSysWsPrivs);
                    dbContext.SaveChanges();

                    dbContext.TbSysWsAccounts.AddRange(accounts);
                    dbContext.TbSysWsModules.AddRange(modules);
                    dbContext.TbSysWsPrivs.AddRange(privileges);
                    dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}

