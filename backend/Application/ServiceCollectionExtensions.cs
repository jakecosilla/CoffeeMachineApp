using Microsoft.Extensions.DependencyInjection;
using Application.Services;
using Application.Interfaces;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<ICoffeeMachineService, CoffeeMachineService>();
            return services;
        }
    }
}
