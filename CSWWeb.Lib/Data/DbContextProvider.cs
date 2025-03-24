using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace CSWWeb.Lib.Data
{
    //動態切換兩個DBContext
    public class DbContextProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DbContext GetDbContext()
        {
            using var scope = _serviceProvider.CreateScope();
            var mssqlDbContext = scope.ServiceProvider.GetRequiredService<WebApiaContext>();

            try
            {
                if (mssqlDbContext.Database.CanConnect())
                {
                    //return _serviceProvider.GetRequiredService<MssqlDbContext>();
                    return _serviceProvider.GetRequiredService<WebApiaContext>();
                }
                else
                {
                    return _serviceProvider.GetRequiredService<SqliteDbContext>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DbContext GetSqliteDbContext()
        {
            return _serviceProvider.GetRequiredService<SqliteDbContext>();
        }
    }
}
