using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Xunit;

namespace Tests.Api.Endpoints;

public class CoffeeEndpointsAprilFoolsTests : IClassFixture<CoffeeTestAprilFoolsWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CoffeeEndpointsAprilFoolsTests(CoffeeTestAprilFoolsWebApplicationFactory factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
        }).CreateClient();
    }

    [Fact]
    public async Task BrewCoffee_OnAprilFools_ReturnsImATeapot()
    {
        // Act
        var response = await _client.GetAsync("/brew-coffee");

        // Assert
        ((int)response.StatusCode).Should().Be(418);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
    }
}
