using CSWWeb.Lib.Data;
using CSWWeb.Lib.Extensions;
using CSWWeb.Lib.Middlewares;
using CSWWeb.Lib.Model;
using CSWWeb.Lib.Utils;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using CSWWeb.Lib.Interface;
using CSWWeb.Lib.Services;
using CSWWeb.Lib;
using CSWWeb.Middlewares;
namespace CSWWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //設定自定義設定檔
            builder.Configuration.AddJsonFile("customsettings.json", optional: true, reloadOnChange: true);
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddScoped<CacheData>();

            // 啟用身份驗證與授權
            builder.Services.AddAuthentication().AddCookie();
            builder.Services.AddAuthorization();
            builder.Services.AddHttpContextAccessor();
            // Add services to the container.
            #region 資料庫相關
            // 配置MSSQL 資料庫
            //builder.Services.AddDbContext<MssqlDbContext>((serviceProvider, options) =>
            builder.Services.AddDbContext<WebApiaContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connString = configuration.GetConnectionString("MssqlConnection");
                options.UseSqlServer(connString);
            });

            // 配置Sqlite資料庫
            builder.Services.AddDbContext<SqliteDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var sqliteSettings = configuration.GetSection("SqliteSettings").Get<SqliteSettings>();
                if (sqliteSettings == null || string.IsNullOrEmpty(sqliteSettings.DbPath))
                {
                    throw new Exception("SQLite 資料庫設定錯誤，請檢查 appsettings.json");
                }

                // Use correct connection string format
                var connectionString = $"Data Source={sqliteSettings.DbPath};Password={sqliteSettings.Password};Pooling = False;";
                options.UseSqlite(connectionString);
            });

            // 註冊 AesEncryptionHelper
            builder.Services.AddSingleton<AesEncryptionHelper>();

            //配置切換DBContext的提供者
            builder.Services.AddScoped<DbContextProvider>();

            #endregion
            // 註冊JwtHelper
            builder.Services.AddScoped<JwtHelper>();

            // 註冊自訂 Logger，例如 MyCustomLogger 必須實作 ICustomLogger
            builder.Services.AddScoped<Lib.Model.TbSysWsLog>();

            // 使其可作為單例服務被呼叫（用於手動觸發同步）
            builder.Services.AddSingleton<BackgroundSyncService>();

            // 註冊背景服務
            builder.Services.AddHostedService(provider => provider.GetRequiredService<BackgroundSyncService>());
            //add memorycache
            builder.Services.AddMemoryCache();
            // Add middleware insert data to cache
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var configuration = builder.Configuration;
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseTTypeMiddleware<LogCountMiddleware>();
            app.UseLoggerMiddleware<LoggerMiddleware<Lib.Model.TbSysWsLog>>();
            app.UseHttpsRedirection();
            app.UseAuthentication(); // 啟用身份驗證
            //引用Basic驗證 - CSWWeb.Lib
            app.UseAuthenticationMiddleware<CustomMiddleware<Lib.Model.TbSysWsAccount>>();
            //引用JWT驗證 - CSWWeb.Lib
            app.UseJWTMiddleware();
            
            app.UseAuthorization();  // 啟用授權
            app.MapControllers();
            app.Run();
        }
    }
}
