using Domain.Entities;

namespace Application.Interfaces;
public interface ICoffeeMachineStateManager
    {
        Task<int> IncrementCallCountAsync();
        Task ResetCountAsync();
        Task<CoffeeMachine> GetStateAsync();
    }