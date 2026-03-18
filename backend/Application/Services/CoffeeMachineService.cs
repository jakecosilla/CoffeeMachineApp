using Application.Interfaces;
using Application.Models;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly ICoffeeMachineStateManager _stateManager;
        private readonly ILogger<CoffeeMachineService> _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CoffeeMachineService(
            ICoffeeMachineStateManager stateManager,
            ILogger<CoffeeMachineService> logger,
            IDateTimeProvider dateTimeProvider)
        {
            _stateManager = stateManager;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<(int StatusCode, CoffeeBrewResponseDto? Response)> BrewCoffeeAsync()
        {
            // Check if it's April 1st (April Fools' Day)
            var today = _dateTimeProvider.Now;
            if (today.Month == 4 && today.Day == 1)
            {
                _logger.LogInformation(Constants.Messages.AprilFoolsLog);
                return (Constants.HttpStatusCodes.ImATeapot, null);
            }

            // Increment call count
            var callCount = await _stateManager.IncrementCallCountAsync();
            _logger.LogInformation(Constants.Messages.CoffeeBrewCallLog, callCount);

            // Every 5th call should return 503 Service Unavailable
            if (callCount % 5 == 0)
            {
                _logger.LogWarning(Constants.Messages.OutOfCoffeeLog, callCount);
                return (Constants.HttpStatusCodes.ServiceUnavailable, null);
            }

            // Normal case: return 200 with coffee ready message
            var response = new CoffeeBrewResponseDto(
                Constants.Messages.CoffeeReadyMessage,
                DateTime.Now.ToString(Constants.Formats.IsoDateFormat));

            _logger.LogInformation(Constants.Messages.CoffeeBrewedSuccessfullyLog, response.Prepared);
            return (Constants.HttpStatusCodes.Ok, response);
        }
    }
}
