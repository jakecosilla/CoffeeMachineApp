using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Tests.Api.Endpoints;

public class CoffeeEndpointsWeatherTests(CoffeeTestHotWeatherFactory factory)
        : IClassFixture<CoffeeTestHotWeatherFactory>
{
    private readonly HttpClient _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
        }).CreateClient();

    [Fact]
    public async Task BrewCoffee_WhenTemperatureAbove30_ReturnsIcedCoffee()
    {
        // Act
        var response = await _client.GetAsync("/brew-coffee");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;

        json.TryGetProperty("message", out var messageProp).Should().BeTrue();
        messageProp.GetString()!.ToLower().Should().Contain("iced coffee");
    }

    [Fact]
    public async Task BrewCoffee_WhenTemperatureIs30OrBelow_ReturnsHotCoffee()
    {
        // Arrange
        var factory = new CoffeeTestColdWeatherFactory();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/brew-coffee");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content).RootElement;

        json.GetProperty("message").GetString()!
            .ToLower().Should().Contain("piping hot coffee");
    }
}
