using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Interface;
using CSWWeb.Lib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CSWWeb.Lib.Middlewares
{
    public class LoggerMiddleware<T>
    where T : class, ICustomLogger
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly string _logkey;

        public LoggerMiddleware(RequestDelegate next, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _next = next;
            _memoryCache = memoryCache;
            _logkey = configuration["MemoryCacheKey:LogKey"];
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // 讀取 Request Body
            string requestBody = await ReadRequestBodyAsync(httpContext);

            // 攔截並捕捉 Response 內容
            string responseText = await CaptureResponseBodyAsync(httpContext);

            // 若 Response 狀態碼不為 200，則進行錯誤 Log
            if (httpContext.Response.StatusCode != 200)
            {
                await LogErrorAsync(httpContext, requestBody, responseText);
            }
        }

        /// <summary>
        /// 讀取 Request Body (僅針對 POST)，並重設 stream 位置
        /// </summary>
        private async Task<string> ReadRequestBodyAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                httpContext.Request.EnableBuffering();
                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                    return body;
                }
            }
            else if (httpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                return httpContext.Request.QueryString.HasValue
                    ? httpContext.Request.QueryString.Value
                    : string.Empty;
            }
            return string.Empty;
        }

        /// <summary>
        /// 攔截 Response.Body 並捕捉回應內容，同時確保資料最終寫回原始 stream
        /// </summary>
        private async Task<string> CaptureResponseBodyAsync(HttpContext httpContext)
        {
            var originalBodyStream = httpContext.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                httpContext.Response.Body = responseBody;
                // 執行後續 middleware 與 endpoint
                await _next(httpContext);
                responseBody.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(responseBody).ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);
                // 將攔截到的內容寫回原始 Response.Body
                await responseBody.CopyToAsync(originalBodyStream);
                httpContext.Response.Body = originalBodyStream;
                return text;
            }
        }

        /// <summary>
        /// 當 Response 狀態碼不為 200 時，記錄 Log 並寫入 DB (或記憶體快取)
        /// </summary>
        private async Task LogErrorAsync(HttpContext httpContext, string requestBody, string responseText)
        {
            string logMessage = $"Context:{requestBody} | HttpStatus:{httpContext.Response.StatusCode} | HttpResult:{responseText}";

            // 取得自訂 Logger 實例
            var logger = httpContext.RequestServices.GetService<T>();
            if (logger != null)
            {
                string userName = httpContext.User.HasClaim(c => c.Type == ClaimTypes.Name)
                    ? httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value
                    : null;

                logger.LogError(httpContext.Response, logMessage, userName);
            }
            // 寫入記憶體快取 (可視需求調整為 DB 寫入)
            if (_memoryCache.TryGetValue(_logkey, out CacheData cacheData) && cacheData != null)
            {
                cacheData.AddItem<T>(logger);
            }
            else
            {
                var newCacheData = new CacheData();
                newCacheData.AddItem<T>(logger);
                _memoryCache.Set(_logkey, newCacheData);
            }
            await Task.CompletedTask;
        }
    }
}
