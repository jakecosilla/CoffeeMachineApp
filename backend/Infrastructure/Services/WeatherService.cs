using System.Net.Http.Json;
using Application.Interfaces;
using Application.Options;
using Application.Models;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherOptions _options;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(
        HttpClient httpClient,
        IOptions<WeatherOptions> options,
        ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<double> GetCurrentTemperatureAsync(CancellationToken cancellationToken = default)
    {
        var url = $"data/2.5/weather?q={_options.City}&units=metric&appid={_options.ApiKey}";

        try
        {
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url, cancellationToken);

            if (response?.Main == null)
            {
                _logger.LogWarning("Weather API returned invalid response: {@Response}", response);
                throw new InvalidOperationException("Invalid weather response");
            }

            return response.Main.Temp;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while calling Weather API");
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Weather API request timed out");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching weather data");
            throw;
        }
    }
}
