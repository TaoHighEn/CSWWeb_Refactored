using Microsoft.EntityFrameworkCore;


namespace CSWWeb.Data
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
                    //throw new Exception();
                    return _serviceProvider.GetRequiredService<SqliteDbContext_2>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
