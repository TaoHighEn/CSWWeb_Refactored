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
        private readonly int _syncInterval;         // 單位：秒
        private readonly int _cacheExpireHour;
        private readonly object _syncLock = new object();
        private readonly string _authKey;
        private readonly string _logKey;
        private readonly string _logCountKey;

        public BackgroundSyncService(
            IServiceProvider serviceProvider,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
            // 若設定為分鐘，可依需求轉換成秒數 (這裡直接轉為秒)
            int.TryParse(configuration["SyncSettings:IntervalMinutes"], out int intervalMinutes);
            //_syncInterval = intervalMinutes > 0 ? intervalMinutes * 60 : 15;
            _syncInterval = intervalMinutes > 0 ? intervalMinutes : 15;
            int.TryParse(configuration["SyncSettings:ExpireHours"], out _cacheExpireHour);

            _authKey = configuration["MemoryCacheKey:AuthKey"];
            _logKey = configuration["MemoryCacheKey:LogKey"];
            _logCountKey = configuration["MemoryCacheKey:LogCountKey"];
        }

        #region Public API

        /// <summary>
        /// 停止同步作業，並記錄活動
        /// </summary>
        public void StopSync()
        {
            _isSyncEnabled = false;
            LogSyncActivity("Stopped", "手動停止同步");
        }

        /// <summary>
        /// 啟動同步作業，並記錄活動，同時立即執行一次同步
        /// </summary>
        public void StartSync()
        {
            _isSyncEnabled = true;
            LogSyncActivity("Started", "手動啟動同步");
            SyncData();
        }

        /// <summary>
        /// 若同步啟用，手動觸發一次同步作業
        /// </summary>
        public void TriggerSync()
        {
            if (_isSyncEnabled)
            {
                SyncData();
            }
        }

        /// <summary>
        /// 回傳目前同步是否啟用的狀態
        /// </summary>
        public bool Status() => _isSyncEnabled;

        #endregion

        #region BackgroundService Override

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 初次啟動時立即同步一次
            SyncData();

            // 設定計時器以定時呼叫 SyncCallback
            _timer = new Timer(SyncCallback, null, TimeSpan.FromSeconds(_syncInterval), TimeSpan.FromSeconds(_syncInterval));

            // 等待直到取消要求
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _timer?.Change(Timeout.Infinite, 0);
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }

        #endregion

        #region Sync Process

        /// <summary>
        /// Timer callback：檢查是否啟用同步，若是則執行同步
        /// </summary>
        private void SyncCallback(object state)
        {
            if (_isSyncEnabled)
            {
                SyncData();
            }
        }

        /// <summary>
        /// 同步作業：更新 MemoryCache、寫入 Log、備份資料、檢查 Sqlite Log
        /// </summary>
        private void SyncData()
        {
            if (!_isSyncEnabled)
                return;

            // 避免重複同步
            if (!Monitor.TryEnter(_syncLock, 0))
                return;

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContextProvider = scope.ServiceProvider.GetRequiredService<DbContextProvider>();
                var dbContext = dbContextProvider.GetDbContext();

                // 從資料庫讀取最新資料
                var (accounts, modules, privileges) = FetchDataFromDb(dbContext);
                string dbType = dbContext.Database.IsSqlServer() ? "MSSQL" : "Sqlite";

                // 更新記憶體快取
                UpdateMemoryCache(accounts, modules, privileges);

                // 將記憶體中累積的 Log 寫入資料庫
                LogProcessData(dbContext);

                // 若使用 MSSQL，備份至 Sqlite 並檢查 Sqlite 是否有遺漏 Log
                if (dbContext.Database.IsSqlServer())
                {
                    BackupToSqlite(accounts, modules, privileges);
                    CheckSqliteLog(dbContext);
                }
            }
            finally
            {
                Monitor.Exit(_syncLock);
            }
        }

        /// <summary>
        /// 從資料庫取得帳號、模組與權限資料
        /// </summary>
        private (List<TbSysWsAccount> accounts, List<TbSysWsModule> modules, List<TbSysWsPriv> privileges)
            FetchDataFromDb(DbContext dbContext)
        {
            var accounts = dbContext.Set<TbSysWsAccount>().ToList();
            var privileges = dbContext.Set<TbSysWsPriv>().ToList();
            var modules = dbContext.Set<TbSysWsModule>().ToList();
            return (accounts, modules, privileges);
        }

        /// <summary>
        /// 更新 MemoryCache，設定指定的到期時間
        /// </summary>
        private void UpdateMemoryCache(List<TbSysWsAccount> accounts, List<TbSysWsModule> modules, List<TbSysWsPriv> privileges)
        {
            var cacheData = new CacheData();
            cacheData.SetList(accounts);
            cacheData.SetList(modules);
            cacheData.SetList(privileges);
            _memoryCache.Set(_authKey, cacheData, TimeSpan.FromHours(_cacheExpireHour));
        }

        /// <summary>
        /// 將累積在 MemoryCache 的 Log 寫入資料庫，並清除 Cache 中的紀錄
        /// </summary>
        private void LogProcessData(DbContext context)
        {
            // 錯誤 Log 紀錄
            if (_memoryCache.TryGetValue(_logKey, out CacheData cacheData) && cacheData != null)
            {
                var errorLogs = cacheData.GetList<TbSysWsLog>();
                context.Set<TbSysWsLog>().AddRange(errorLogs);
            }
            // API 呼叫 Log 紀錄
            if (_memoryCache.TryGetValue(_logCountKey, out CacheData cacheDataLog) && cacheDataLog != null)
            {
                var apiLogs = cacheDataLog.GetList<TbSysApiApplylog>();
                foreach (var log in apiLogs)
                {
                    context.Add(log);
                    // 同步更新對應模組的最後呼叫時間
                    var module = context.Set<TbSysWsModule>().FirstOrDefault(x => x.WsModule == log.Module && x.WsMethod == log.Method);
                    if (module != null)
                    {
                        module.Datestamp = log.Lastdatetime;
                        context.Update(module);
                    }
                }
            }
            // 儲存至資料庫（同步處理）
            context.SaveChanges();
            _memoryCache.Remove(_logKey);
            _memoryCache.Remove(_logCountKey);
        }

        /// <summary>
        /// 備份資料至 Sqlite：清除舊有資料後重新寫入
        /// </summary>
        private void BackupToSqlite(List<TbSysWsAccount> accounts, List<TbSysWsModule> modules, List<TbSysWsPriv> privileges)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sqliteContext = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                Console.WriteLine($"[INFO] Using database: {sqliteContext.Database.GetDbConnection().ConnectionString}");
                sqliteContext.Database.EnsureCreated();

                // 清除現有資料
                sqliteContext.TbSysWsAccounts.RemoveRange(sqliteContext.TbSysWsAccounts);
                sqliteContext.TbSysWsModules.RemoveRange(sqliteContext.TbSysWsModules);
                sqliteContext.TbSysWsPrivs.RemoveRange(sqliteContext.TbSysWsPrivs);
                sqliteContext.SaveChanges();

                // 寫入新資料
                sqliteContext.TbSysWsAccounts.AddRange(accounts);
                sqliteContext.TbSysWsModules.AddRange(modules);
                sqliteContext.TbSysWsPrivs.AddRange(privileges);
                sqliteContext.SaveChanges();
            }
        }

        /// <summary>
        /// 檢查 Sqlite 是否有遺漏的 Log，若有則存回 MSSQL，再清除 Sqlite 中的紀錄
        /// </summary>
        private void CheckSqliteLog(DbContext mssqlContext)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var sqliteContext = scope.ServiceProvider.GetRequiredService<SqliteDbContext>();
                    if (sqliteContext.TbSysWsLogs.Any())
                    {
                        // 讀取 Sqlite 中的 Log 與 API 呼叫紀錄
                        var wsLogs = sqliteContext.TbSysWsLogs.AsNoTracking().ToList();
                        var apiLogs = sqliteContext.TbSysApiApplylog.AsNoTracking().ToList();

                        // 重置序號，確保新增至 MSSQL 時為新資料
                        wsLogs.ForEach(x => x.Seq = 0);
                        apiLogs.ForEach(x => x.Seq = 0);

                        mssqlContext.Set<TbSysWsLog>().AddRange(wsLogs);
                        mssqlContext.Set<TbSysApiApplylog>().AddRange(apiLogs);
                        mssqlContext.SaveChanges();

                        // 清除 Sqlite 中的舊紀錄
                        sqliteContext.TbSysWsLogs.ExecuteDelete();
                        sqliteContext.TbSysApiApplylog.ExecuteDelete();
                        sqliteContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                // 此處可依需求記錄 Log 或採取其他錯誤處理措施
                throw;
            }
        }

        #endregion

        #region Log Activity

        /// <summary>
        /// 記錄同步活動到資料庫
        /// </summary>
        private void LogSyncActivity(string status, string description, string source = "系統")
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContextProvider = scope.ServiceProvider.GetRequiredService<DbContextProvider>();
                    var dbContext = dbContextProvider.GetDbContext();
                    string dbType = dbContext.Database.IsSqlServer() ? "MSSQL" : "Sqlite";

                    dbContext.Set<TbSysWsLog>().Add(new TbSysWsLog
                    {
                        WsAp = "System",
                        WsModule = "Sync",
                        LogMessage = $"同步資料 Log From {dbType} - {description}",
                        LogType = "INFO",
                        LogStatus = status,
                        Datestamp = DateTime.UtcNow
                    });
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // 可在此記錄例外狀況
                throw;
            }
        }
        #endregion
    }
}

