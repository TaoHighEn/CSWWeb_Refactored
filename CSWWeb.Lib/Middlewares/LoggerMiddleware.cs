using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Interface;
using CSWWeb.Lib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace CSWWeb.Lib.Middlewares
{
    public class LoggerMiddleware<TDbContext, T>
    where TDbContext : DbContext
    where T : class, ICustomLogger
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMemoryCache _memoryCache;

        public LoggerMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _next = next;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 產生 Log 訊息
            string logMessage = $"API Called: {context.Request.Path} | Method: {context.Request.Method} | IP: {context.Connection.RemoteIpAddress}";

            // 取得自訂 Logger 實例
            var logger = context.RequestServices.GetService<T>();
            if (logger != null)
            {
                var wsap = context.User.HasClaim(c => c.Type == ClaimTypes.Name) ? context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value : null;
                logger.Log(context.Request.Path, logMessage, wsap);
            }
            if (_memoryCache.TryGetValue("ApiLog", out CacheData cachedata) && cachedata != null)
            {
                // 已存在 CacheData，就直接新增 logger 到指定型別的 List 中
                cachedata.AddItem<T>(logger);
            }
            else
            {
                // 不存在，先建立一個新的 CacheData 實例
                var newCacheData = new CacheData();
                newCacheData.AddItem<T>(logger);
                // 將新建立的 CacheData 實例存入 MemoryCache
                _memoryCache.Set("ApiLog", newCacheData);
            }
            // 將 Log 訊息暫存到 MemoryCache (或其他備查方式)
            //_memoryCache.Set("LastApiLog", logMessage, TimeSpan.FromMinutes(5));

            // 若需要可在此同步寫入資料庫
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
                // dbContext.Set<TbSysWsLog>().Add(...);
                // await dbContext.SaveChangesAsync();
            }

            await _next(context);
        }
    }
}
