using CSWWeb.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CSWWeb.Data
{
    public partial class SqliteDbContext_2 : WebApiaContext
    {
        private readonly IConfiguration _configuration;

        public SqliteDbContext_2(DbContextOptions<WebApiaContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured
            var sqliteSettings = _configuration.GetSection("SqliteSettings").Get<SqliteSettings>();
            if (sqliteSettings == null || string.IsNullOrEmpty(sqliteSettings.DbPath))
            {
                throw new Exception("SQLite 資料庫設定錯誤，請檢查 appsettings.json");
            }
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = $"Data Source={sqliteSettings.DbPath}";
                var connection = new SqliteConnection(connectionString);
                connection.Open();

                // Enable SQLCipher extensions
                connection.EnableExtensions(true);

                // Set password using PRAGMA
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"PRAGMA key = '{sqliteSettings.Password}'";
                    command.ExecuteNonQuery();
                }

                optionsBuilder.UseSqlite(connection);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
