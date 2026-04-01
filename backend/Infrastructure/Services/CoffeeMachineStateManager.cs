using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Services;

    /// <summary>
    /// Manages the state of the coffee machine, including call tracking.
    /// This is an in-memory implementation for simplicity.
    /// </summary>
public class CoffeeMachineStateManager : ICoffeeMachineStateManager
{
    private CoffeeMachine _state = new CoffeeMachine
    {
        CallCount = 0,
        LastReset = DateTime.Now
    };

    private readonly object _lockObject = new object();

    public Task<int> IncrementCallCountAsync()
    {
        lock (_lockObject)
        {
            _state.CallCount++;
            return Task.FromResult(_state.CallCount);
        }
    }

    public Task ResetCountAsync()
    {
        lock (_lockObject)
        {
            _state.CallCount = 0;
            _state.LastReset = DateTime.Now;
            return Task.CompletedTask;
        }
    }

    public Task<CoffeeMachine> GetStateAsync()
    {
        lock (_lockObject)
        {
            return Task.FromResult(new CoffeeMachine
            {
                CallCount = _state.CallCount,
                LastReset = _state.LastReset
            });
        }
    }
}

