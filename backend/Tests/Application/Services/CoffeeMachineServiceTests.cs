using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Application.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Infrastructure.Services;

namespace Tests.Application.Services;

public class CoffeeMachineServiceTests
{
    private IDateTimeProvider CreateDateTimeProvider(int month = 1, int day = 15)
    {
        var mock = new Mock<IDateTimeProvider>();
        mock.Setup(x => x.Now).Returns(new DateTimeOffset(2025, month, day, 10, 0, 0, TimeSpan.FromHours(8)));
        return mock.Object;
    }

    private IWeatherService CreateWeatherService(double temp = 25, bool shouldFail = false)
    {
        var mock = new Mock<IWeatherService>();

        if (shouldFail)
        {
            mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Weather failed"));
        }
        else
        {
            mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(temp);
        }

        return mock.Object;
    }

    private CoffeeMachineService CreateService(
        IDateTimeProvider dateTimeProvider,
        IWeatherService weatherService,
        ICoffeeMachineStateManager? stateManager = null)
    {
        stateManager ??= new CoffeeMachineStateManager();
        var logger = new Mock<ILogger<CoffeeMachineService>>();

        return new CoffeeMachineService(
            stateManager,
            logger.Object,
            dateTimeProvider,
            weatherService);
    }

    [Fact]
    public async Task BrewCoffeeAsync_FirstCall_ReturnsHotCoffee()
    {
        var service = CreateService(
            CreateDateTimeProvider(),
            CreateWeatherService(25));

        var (statusCode, response) = await service.BrewCoffeeAsync();

        statusCode.Should().Be(200);
        response.Should().NotBeNull();
        response!.Value.Message.Should().Be("Your piping hot coffee is ready");
    }

    [Fact]
    public async Task BrewCoffeeAsync_WhenTemperatureAbove30_ReturnsIcedCoffee()
    {
        var service = CreateService(
            CreateDateTimeProvider(),
            CreateWeatherService(35));

        var (_, response) = await service.BrewCoffeeAsync();

        response!.Value.Message.ToLower().Should().Contain("iced coffee");
    }

    [Fact]
    public async Task BrewCoffeeAsync_WhenWeatherFails_FallsBackToHotCoffee()
    {
        var service = CreateService(
            CreateDateTimeProvider(),
            CreateWeatherService(shouldFail: true));

        var (_, response) = await service.BrewCoffeeAsync();

        response!.Value.Message.ToLower().Should().Contain("coffee");
    }

    [Fact]
    public async Task BrewCoffeeAsync_Every5thCall_Returns503()
    {
        var service = CreateService(
            CreateDateTimeProvider(),
            CreateWeatherService());

        for (int i = 1; i <= 4; i++)
        {
            var (statusCode, _) = await service.BrewCoffeeAsync();
            statusCode.Should().Be(200);
        }

        var (fifthStatus, fifthResponse) = await service.BrewCoffeeAsync();

        fifthStatus.Should().Be(503);
        fifthResponse.Should().BeNull();
    }

    [Fact]
    public async Task BrewCoffeeAsync_OnAprilFools_Returns418()
    {
        var service = CreateService(
            CreateDateTimeProvider(4, 1),
            CreateWeatherService());

        var (statusCode, response) = await service.BrewCoffeeAsync();

        statusCode.Should().Be(418);
        response.Should().BeNull();
    }
}
