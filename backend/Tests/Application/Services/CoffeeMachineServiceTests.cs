using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Interfaces;
using Application.Services;
using FluentAssertions;
using Moq;
using Xunit;
using Infrastructure.Services;

namespace Tests.Application.Services
{
    public class CoffeeMachineServiceTests
    {
        private IDateTimeProvider CreateDateTimeProvider(int month = 1, int day = 15)
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.Now).Returns(new DateTime(2025, month, day, 10, 0, 0));
            return dateTimeProviderMock.Object;
        }

        [Fact]
        public async Task BrewCoffeeAsync_FirstCall_Returns200WithMessage()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act
            var (statusCode, response) = await service.BrewCoffeeAsync();

            // Assert
            statusCode.Should().Be(200);
            response.Should().NotBeNull();
            response!.Value.Message.Should().Be("Your piping hot coffee is ready");
            response!.Value.Prepared.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task BrewCoffeeAsync_FirstCall_ReturnsValidISO8601Timestamp()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act
            var (_, response) = await service.BrewCoffeeAsync();

            // Assert
            response.Should().NotBeNull();
            response!.Value.Prepared.Should().NotBeNullOrEmpty();
            
            // Should be parseable as DateTime
            Action act = () => DateTime.Parse(response!.Value.Prepared);
            act.Should().NotThrow();
            
            // Should match ISO-8601 format with timezone info
            response!.Value.Prepared.Should().MatchRegex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?[+-]\d{2}:\d{2}");
        }

        [Fact]
        public async Task BrewCoffeeAsync_Every5thCall_Returns503()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act & Assert - calls 1-4 return 200
            for (int i = 1; i <= 4; i++)
            {
                var (statusCode, _) = await service.BrewCoffeeAsync();
                statusCode.Should().Be(200, $"Call {i} should return 200");
            }

            // Act - 5th call
            var (fifthStatus, fifthResponse) = await service.BrewCoffeeAsync();

            // Assert - 5th call returns 503
            fifthStatus.Should().Be(503);
            fifthResponse.Should().BeNull();
        }

        [Fact]
        public async Task BrewCoffeeAsync_10thCall_Returns503()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act - make 9 calls
            for (int i = 1; i <= 9; i++)
            {
                await service.BrewCoffeeAsync();
            }

            // Act - 10th call
            var (statusCode, response) = await service.BrewCoffeeAsync();

            // Assert
            statusCode.Should().Be(503);
            response.Should().BeNull();
        }

        [Fact]
        public async Task BrewCoffeeAsync_CallAfter5th_ResumesWithOk()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act - make 5 calls (5th is 503)
            for (int i = 1; i <= 5; i++)
            {
                await service.BrewCoffeeAsync();
            }

            // Act - 6th call
            var (statusCode, response) = await service.BrewCoffeeAsync();

            // Assert
            statusCode.Should().Be(200);
            response.Should().NotBeNull();
            response!.Value.Message.Should().Be("Your piping hot coffee is ready");
        }

        [Fact]
        public async Task BrewCoffeeAsync_12ConsecutiveCalls_FollowsExpectedPattern()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider();
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act - make 12 calls
            var statuses = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                var (statusCode, _) = await service.BrewCoffeeAsync();
                statuses.Add(statusCode);
            }

            // Assert - verify pattern: 200, 200, 200, 200, 503, 200, 200, 200, 200, 503, 200, 200
            statuses.Should().HaveCount(12);
            statuses[0].Should().Be(200);   // 1st
            statuses[1].Should().Be(200);   // 2nd
            statuses[2].Should().Be(200);   // 3rd
            statuses[3].Should().Be(200);   // 4th
            statuses[4].Should().Be(503);   // 5th
            statuses[5].Should().Be(200);   // 6th
            statuses[6].Should().Be(200);   // 7th
            statuses[7].Should().Be(200);   // 8th
            statuses[8].Should().Be(200);   // 9th
            statuses[9].Should().Be(503);   // 10th
            statuses[10].Should().Be(200);  // 11th
            statuses[11].Should().Be(200);  // 12th
        }

        [Fact]
        public async Task BrewCoffeeAsync_OnAprilFools_Returns418ImATeapot()
        {
            // Arrange
            var stateManager = new CoffeeMachineStateManager();
            var loggerMock = new Mock<ILogger<CoffeeMachineService>>();
            var dateTimeProvider = CreateDateTimeProvider(month: 4, day: 1);
            var service = new CoffeeMachineService(stateManager, loggerMock.Object, dateTimeProvider);

            // Act
            var (statusCode, response) = await service.BrewCoffeeAsync();

            // Assert
            statusCode.Should().Be(418);
            response.Should().BeNull();
        }
    }
}
