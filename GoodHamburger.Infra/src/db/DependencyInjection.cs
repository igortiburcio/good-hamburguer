using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GoodHamburger.Infra.Src.Db.Repositories;
using GoodHamburger.Infra.Src.Db.Seeds;
using GoodHamburger.Application.Src.Repositories;

namespace GoodHamburger.Infra.Src.Db;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfraDatabase(configuration);
        services.AddInfraRepositories();

        return services;
    }

    public static IServiceCollection AddInfraDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddInfraRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IComboRepository, ComboRepository>();

        return services;
    }

    public static IServiceProvider InitializeInfra(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        dbContext.Database.Migrate();
        DatabaseSeeder.SeedProducts(dbContext);

        return serviceProvider;
    }
}
