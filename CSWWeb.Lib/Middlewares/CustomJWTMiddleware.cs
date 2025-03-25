using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CSWWeb.Lib.Middlewares
{
    /// <summary>
    /// 建立JWT驗證機制
    /// </summary>
    public class CustomJWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCache;
        private readonly string _secretKey;
        private readonly string _authkey;

        public CustomJWTMiddleware(RequestDelegate next, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _next = next;
            _secretKey = configuration["Jwt:Key"] ?? "";
            _authkey = configuration["MemoryCacheKey:AuthKey"] ?? "";
            _memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            _memoryCache.TryGetValue(_authkey, out CacheData cacheData);
            if (cacheData == null)
            {
                context.Response.StatusCode = 500; // 錯誤請求
                await context.Response.WriteAsync("Initial Data Loss");
                return;
            }

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                {
                    AttachUserToContext(context, token);
                }
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = System.TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                context.User = principal;
            }
            catch
            {
                throw;
            }
        }
    }
}
