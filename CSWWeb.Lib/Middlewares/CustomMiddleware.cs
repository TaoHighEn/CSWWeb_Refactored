using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using CSWWeb.Lib.Interface;
using CSWWeb.Lib.Model;
using CSWWeb.Lib.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace CSWWeb.Lib.Middlewares
{
    /// <summary>
    /// Default Basic Auth Valid Middleware
    /// </summary>
    public class CustomMiddleware<T> where T : class, IUserValidatable
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoData;
        private readonly AesEncryptionHelper _aesEncryptionHelper;
        private readonly string _authkey;
        public CustomMiddleware(RequestDelegate next, IMemoryCache memoData,AesEncryptionHelper aesEncryptionHelper, IConfiguration configuration)
        {
            _next = next;
            _memoData = memoData;
            _aesEncryptionHelper = aesEncryptionHelper;
            _authkey = configuration["MemoryCacheKey:AuthKey"] ?? "";
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                _memoData.TryGetValue(_authkey, out CacheData _cacheData);
                if (_cacheData == null)
                {
                    context.Response.StatusCode = 500; // 錯誤請求
                    await context.Response.WriteAsync("Initial Data Loss");
                    return;
                }
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // 解析帳號密碼
                    var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(':');
                    if (credentials.Length != 2)
                    {
                        context.Response.StatusCode = 400; // 錯誤請求
                        await context.Response.WriteAsync("Invalid Authorization Header");
                        return;
                    }

                    var username = credentials[0];
                    var password = _aesEncryptionHelper.EncryptString(credentials[1]);

                    // 驗證帳號密碼
                    var accountInfo = _cacheData.GetList<T>().FirstOrDefault(x => x.ValidateUser(username, password));
                    if (accountInfo == null)
                    {
                        context.Response.StatusCode = 403; // 禁止存取
                        await context.Response.WriteAsync("Forbidden - Invalid Authorization.");
                        return;
                    }

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    var principal = new ClaimsPrincipal(identity);

                    context.User = principal;
                }
                // 繼續處理請求
                await _next(context);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
