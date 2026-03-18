using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Api.Endpoints
{
    public static class CoffeeEndpoints
    {
        public static WebApplication MapCoffeeEndpoints(this WebApplication app)
        {
            app.MapGet("/brew-coffee", BrewCoffee)
                .WithName("BrewCoffee")
                .WithOpenApi()
                .Produces(200, typeof(CoffeeBrewResponseDto))
                .Produces(503)
                .Produces(418);

            return app;
        }

        public static async Task<IResult> BrewCoffee(ICoffeeMachineService coffeeMachineService)
        {
            var (statusCode, response) = await coffeeMachineService.BrewCoffeeAsync();

            return statusCode switch
            {
                200 => Results.Ok(response),
                503 => Results.StatusCode(503),
                418 => Results.StatusCode(418),
                _ => Results.BadRequest()
            };
        }
    }
}
