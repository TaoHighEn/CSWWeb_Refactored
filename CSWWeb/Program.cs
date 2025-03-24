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
namespace CSWWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //�]�w�۩w�q�]�w��
            builder.Configuration.AddJsonFile("customsettings.json", optional: true, reloadOnChange: true);
            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            builder.Services.AddSingleton<CacheData>();

            // �ҥΨ������һP���v
            builder.Services.AddAuthentication().AddCookie();
            builder.Services.AddAuthorization();
            // Add services to the container.
            #region ��Ʈw����
            // �t�mMSSQL ��Ʈw
            //builder.Services.AddDbContext<MssqlDbContext>((serviceProvider, options) =>
            builder.Services.AddDbContext<WebApiaContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var connString = configuration.GetConnectionString("MssqlConnection");
                options.UseSqlServer(connString);
            });
            // �t�mSqlite��Ʈw
            builder.Services.AddDbContext<SqliteDbContext>((serviceProvider, options) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var sqliteSettings = configuration.GetSection("SqliteSettings").Get<SqliteSettings>();
                if (sqliteSettings == null || string.IsNullOrEmpty(sqliteSettings.DbPath))
                {
                    throw new Exception("SQLite ��Ʈw�]�w���~�A���ˬd appsettings.json");
                }

                // Use correct connection string format
                var connectionString = $"Data Source={sqliteSettings.DbPath};Password={sqliteSettings.Password}";
                options.UseSqlite(connectionString);
            });
            //�t�m����DBContext�����Ѫ�
            builder.Services.AddScoped<DbContextProvider>();
            #endregion
            // ���UJwtHelper
            builder.Services.AddSingleton<JwtHelper>();

            // ���U�ۭq Logger�A�Ҧp MyCustomLogger ������@ ICustomLogger
            builder.Services.AddScoped<ICustomLogger, Lib.Model.TbSysWsLog>();
            // �Ϩ�i�@����ҪA�ȳQ�I�s�]�Ω���Ĳ�o�P�B�^
            builder.Services.AddSingleton<BackgroundSyncService>();
            // ���U�I���A��
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
            // **��l�� MemoryCache ���ո��**
            using (var scope = app.Services.CreateScope())
            {
                var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
                
                // �إ� CacheData ���
                var cacheData = new CacheData();
                cacheData.AddItem<Lib.Model.TbSysWsAccount>(new Lib.Model.TbSysWsAccount() { Sys = "User", SysPwd = "P@ssw0rd" });
                cacheData.AddItem<Lib.Model.TbSysWsAccount>(new Lib.Model.TbSysWsAccount() { Sys = "User2", SysPwd = new AesEncryptionHelper(configuration).EncryptString("P@ssw0rd") });
                cacheData.AddItem<Lib.Model.TbSysWsAccount>(new Lib.Model.TbSysWsAccount() { Sys = "User3", SysPwd = "P@ssw0rd" });
                memoryCache.Set("Auth", cacheData);
            }

            app.UseHttpsRedirection();
            app.UseAuthentication(); // �ҥΨ�������
            //�ޥ�Basic���� - CSWWeb.Lib
            app.UseAuthenticationMiddleware<CustomMiddleware<Lib.Model.TbSysWsAccount>>();
            //�ޥ�JWT���� - CSWWeb.Lib
            app.UseJWTMiddleware();
            //�ۭq��������Middleware
            //app.UseAuthenticationMiddleware<YourOwnerMiddleware>();
            using (var scope = app.Services.CreateScope())
            {
                // ���o DbContextProvider ���
                var provider = scope.ServiceProvider.GetRequiredService<Lib.Data.DbContextProvider>();
                // �ھڳs�u���A���o��ڪ� DbContext
                var dbContext = provider.GetDbContext();

                // �̾ڹ�ڦ^�Ǫ� DbContext ���O�Өϥ� LoggerMiddleware
                if (dbContext is Lib.Data.WebApiaContext)
                {
                    app.UseLoggerMiddleware<Lib.Data.WebApiaContext, ICustomLogger>();
                }
                else if (dbContext is Lib.Data.SqliteDbContext)
                {
                    app.UseLoggerMiddleware<Lib.Data.SqliteDbContext, ICustomLogger>();
                }
            }
            app.UseAuthorization();  // �ҥα��v
            app.MapControllers();
            app.Run();
        }
    }
}
