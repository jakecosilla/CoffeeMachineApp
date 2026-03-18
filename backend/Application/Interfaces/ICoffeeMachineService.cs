using Application.Models;

namespace Application.Interfaces
{
    public interface ICoffeeMachineService
    {
        Task<(int StatusCode, CoffeeBrewResponseDto? Response)> BrewCoffeeAsync();
    }
}
