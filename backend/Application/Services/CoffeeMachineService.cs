using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CoffeeMachineService : ICoffeeMachineService
{
    private readonly ICoffeeMachineStateManager _stateManager;
    private readonly ILogger<CoffeeMachineService> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IWeatherService _weatherService;

    public CoffeeMachineService(
        ICoffeeMachineStateManager stateManager,
        ILogger<CoffeeMachineService> logger,
        IDateTimeProvider dateTimeProvider,
        IWeatherService weatherService)
    {
        _stateManager = stateManager;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _weatherService = weatherService;
    }

    public async Task<(int StatusCode, CoffeeBrewResponseDto? Response)> BrewCoffeeAsync()
    {
        var today = _dateTimeProvider.Now;

        // 1. April Fools first (highest priority)
        if (today.Month == 4 && today.Day == 1)
        {
            _logger.LogInformation(Constants.Messages.AprilFoolsLog);
            return (Constants.HttpStatusCodes.ImATeapot, null);
        }

        // 2. Increment call count
        var callCount = await _stateManager.IncrementCallCountAsync();
        _logger.LogInformation(Constants.Messages.CoffeeBrewCallLog, callCount);

        // 3. Every 5th call → 503
        if (callCount % 5 == 0)
        {
            _logger.LogWarning(Constants.Messages.OutOfCoffeeLog, callCount);
            return (Constants.HttpStatusCodes.ServiceUnavailable, null);
        }

        // 4. Weather check (NEW FEATURE)
        string message = Constants.Messages.CoffeeReadyMessage;

        try
        {
            var temperature = await _weatherService.GetCurrentTemperatureAsync(default);

            if (temperature > 30)
            {
                message = Constants.Messages.IcedCoffeeMessage;
            }
        }
        catch (Exception ex)
        {
            // IMPORTANT: Do NOT break coffee machine if weather fails
            _logger.LogError(ex, "Weather service failed. Defaulting to hot coffee.");
        }

        // 5. Response
        var response = new CoffeeBrewResponseDto(
            message,
            _dateTimeProvider.Now.ToString(Constants.Formats.IsoDateFormat));

        _logger.LogInformation(Constants.Messages.CoffeeBrewedSuccessfullyLog, response.Prepared);

        return (Constants.HttpStatusCodes.Ok, response);
    }
}
