using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSWWeb.Lib.Interface;
using CSWWeb.Lib.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace CSWWeb.Lib.Extensions
{
    public static class CustomMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthenticationMiddleware<T>(this IApplicationBuilder app) where T : class
        {
            return app.UseMiddleware<T>();
        }

        public static IApplicationBuilder UseJWTMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CustomJWTMiddleware>();
        }
        // 這個方法讓主程式能夠注入自訂 LoggerMiddleware，
        // 其中 TLogger 必須實作 ICustomLogger
        public static IApplicationBuilder UseLoggerMiddleware<TDbContext, TLogger>(this IApplicationBuilder app)
            where TDbContext : DbContext
            where TLogger : class , ICustomLogger
        {
            return app.UseMiddleware<LoggerMiddleware<TDbContext, TLogger>>();
        }
    }
}
