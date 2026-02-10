using ChannelsService.External.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelsService.External;

public static class DependencyInjectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddDatabase() =>
            services.AddDbContext<DatabaseContext>((provider, options) =>
            {
                IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
                
                string connectionString = configuration.GetConnectionString("Database")
                    ?? throw new ArgumentException("Database connection string doesn't exist");
                
                options.UseNpgsql(connectionString);
            });
    }
}