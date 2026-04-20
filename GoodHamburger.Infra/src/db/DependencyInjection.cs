using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Infra.Src.Db;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
