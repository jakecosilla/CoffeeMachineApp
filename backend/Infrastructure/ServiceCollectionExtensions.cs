using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;

namespace Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ICoffeeMachineStateManager, CoffeeMachineStateManager>();
            return services;
        }
    }
}
