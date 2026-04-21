using GoodHamburger.Application.Src.UseCases;
using GoodHamburger.Domain.Src;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application.Src;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        services.AddScoped<MenuUseCase>();
        services.AddScoped<OrderUseCase>();
        services.AddScoped<DiscountCalculator>();

        return services;
    }
}
