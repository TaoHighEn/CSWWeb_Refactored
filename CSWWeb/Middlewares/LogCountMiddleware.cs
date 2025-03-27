using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace CSWWeb.Middlewares
{
    public class LogCountMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly string _logCountKey;

        public LogCountMiddleware(RequestDelegate next, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _next = next;
            _memoryCache = memoryCache;
            _logCountKey = configuration["MemoryCacheKey:LogCountKey"];
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // 先執行後續 pipeline
            await _next(httpContext);

            // 建立 API 呼叫資訊記錄
            var logRecord = BuildLogRecord(httpContext);

            // 更新記錄到 MemoryCache
            UpdateLogCount(logRecord);
        }

        /// <summary>
        /// 根據 HttpContext 建立 TbSysApiApplylog 實例
        /// </summary>
        private TbSysApiApplylog BuildLogRecord(HttpContext httpContext)
        {
            var sys = httpContext.User.Identity?.Name ?? "Unrestricted";

            // 從路由值中取得模組與方法資訊，若無則給予預設值
            string module = httpContext.Request.RouteValues.TryGetValue("controller", out var controller)
                ? controller?.ToString() ?? "Unknown"
                : "Unknown";

            string method = httpContext.Request.RouteValues.TryGetValue("action", out var action)
                ? action?.ToString() ?? "Unknown"
                : "Unknown";

            return new TbSysApiApplylog
            {
                Sys = sys,
                Module = module,
                Method = method,
                Count = 1,
                Lastdatetime = DateTime.UtcNow.AddHours(8)
            };
        }

        /// <summary>
        /// 更新或加入 API 呼叫計數到 MemoryCache
        /// </summary>
        private void UpdateLogCount(TbSysApiApplylog newLog)
        {
            if (_memoryCache.TryGetValue(_logCountKey, out CacheData cacheData) && cacheData != null)
            {
                var existingLogs = cacheData.GetList<TbSysApiApplylog>();
                var log = existingLogs.FirstOrDefault(x =>
                    x.Sys == newLog.Sys &&
                    x.Module == newLog.Module &&
                    x.Method == newLog.Method);

                if (log != null)
                {
                    // 如果已存在相同記錄，累加計數並更新最後更新時間
                    log.Count = (log.Count ?? 0) + 1;
                    log.Lastdatetime = DateTime.UtcNow.AddHours(8);
                }
                else
                {
                    // 不存在則直接加入新的 log
                    cacheData.AddItem<TbSysApiApplylog>(newLog);
                }
            }
            else
            {
                // 若 MemoryCache 中尚無 CacheData，則建立新的 CacheData 實例並加入 log
                var newCacheData = new CacheData();
                newCacheData.AddItem(newLog);
                _memoryCache.Set(_logCountKey, newCacheData);
            }
        }
    }
}
