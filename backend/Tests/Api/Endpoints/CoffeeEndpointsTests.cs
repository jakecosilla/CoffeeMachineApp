using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Api;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Api.Endpoints
{
    public class CoffeeEndpointsTests : IClassFixture<CoffeeTestWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public CoffeeEndpointsTests(CoffeeTestWebApplicationFactory factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
            }).CreateClient();
        }

        [Fact]
        public async Task BrewCoffee_FirstCall_ReturnsOkWithBrewedCoffee()
        {
            // Act
            var response = await _client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            root.TryGetProperty("message", out var messageProp).Should().BeTrue();
            messageProp.GetString().Should().Be("Your piping hot coffee is ready");

            root.TryGetProperty("prepared", out var preparedProp).Should().BeTrue();
            var preparedStr = preparedProp.GetString();
            preparedStr.Should().NotBeNullOrEmpty();

            // Verify ISO-8601 format with timezone
            preparedStr!.Should().MatchRegex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?[+-]\d{2}:\d{2}");
        }

        [Fact]
        public async Task BrewCoffee_MultipleConsecutiveCalls_Every5thReturnsServiceUnavailable()
        {
            // Arrange & Act & Assert - calls 1-4 return 200
            for (int i = 1; i <= 4; i++)
            {
                var response = await _client.GetAsync("/brew-coffee");
                response.StatusCode.Should().Be(HttpStatusCode.OK, $"Call {i} should return 200");
            }

            // Act - 5th call
            var fifthResponse = await _client.GetAsync("/brew-coffee");

            // Assert - 5th call returns 503
            fifthResponse.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            var fifthContent = await fifthResponse.Content.ReadAsStringAsync();
            fifthContent.Should().BeEmpty();
        }

        [Fact]
        public async Task BrewCoffee_10thCall_ReturnsServiceUnavailable()
        {
            // Arrange - make 9 calls
            for (int i = 1; i <= 9; i++)
            {
                await _client.GetAsync("/brew-coffee");
            }

            // Act - 10th call
            var response = await _client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        }

        [Fact]
        public async Task BrewCoffee_After5thCall_ResumesWithOk()
        {
            // Arrange - make 5 calls (5th is 503)
            for (int i = 1; i <= 5; i++)
            {
                await _client.GetAsync("/brew-coffee");
            }

            // Act - 6th call
            var response = await _client.GetAsync("/brew-coffee");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            jsonDoc.RootElement.TryGetProperty("message", out _).Should().BeTrue();
        }

        [Fact]
        public async Task BrewCoffee_12ConsecutiveCalls_FollowsExpectedPattern()
        {
            // Act - make 12 calls
            var statuses = new List<HttpStatusCode>();
            for (int i = 1; i <= 12; i++)
            {
                var response = await _client.GetAsync("/brew-coffee");
                statuses.Add(response.StatusCode);
            }

            // Assert - verify pattern: 200, 200, 200, 200, 503, 200, 200, 200, 200, 503, 200, 200
            statuses[0].Should().Be(HttpStatusCode.OK);    // 1st
            statuses[1].Should().Be(HttpStatusCode.OK);    // 2nd
            statuses[2].Should().Be(HttpStatusCode.OK);    // 3rd
            statuses[3].Should().Be(HttpStatusCode.OK);    // 4th
            statuses[4].Should().Be(HttpStatusCode.ServiceUnavailable); // 5th
            statuses[5].Should().Be(HttpStatusCode.OK);    // 6th
            statuses[6].Should().Be(HttpStatusCode.OK);    // 7th
            statuses[7].Should().Be(HttpStatusCode.OK);    // 8th
            statuses[8].Should().Be(HttpStatusCode.OK);    // 9th
            statuses[9].Should().Be(HttpStatusCode.ServiceUnavailable); // 10th
            statuses[10].Should().Be(HttpStatusCode.OK);   // 11th
            statuses[11].Should().Be(HttpStatusCode.OK);   // 12th
        }

        [Fact]
        public async Task BrewCoffee_ResponseIsValidJson()
        {
            // Act
            var response = await _client.GetAsync("/brew-coffee");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var act = () => JsonDocument.Parse(content);
            act.Should().NotThrow();
        }
    }
}
