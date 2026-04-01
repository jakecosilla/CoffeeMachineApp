using Application.Interfaces;
using Application.Options;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind WeatherOptions
        services.AddOptions<WeatherOptions>()
            .Bind(configuration.GetSection("Weather"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "Weather ApiKey is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "Weather BaseUrl is required")
            .ValidateOnStart();

        services.AddHttpClient<IWeatherService, WeatherService>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<WeatherOptions>>().Value;

            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(3);
        });

        services.AddSingleton<ICoffeeMachineStateManager, CoffeeMachineStateManager>();

        return services;
    }
}
