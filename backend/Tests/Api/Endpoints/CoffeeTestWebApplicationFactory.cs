using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Api.Endpoints;

public class CoffeeTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected virtual DateTimeOffset MockDate => new DateTimeOffset(2025, 1, 15, 10, 0, 0, TimeSpan.FromHours(8));

    protected virtual IWeatherService CreateWeatherServiceMock()
    {
        var mock = new Mock<IWeatherService>();

        // Default behavior (safe fallback)
        mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        return mock.Object;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace DateTimeProvider
            var dateDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDateTimeProvider));
            if (dateDescriptor != null)
                services.Remove(dateDescriptor);

            var dateMock = new Mock<IDateTimeProvider>();
            dateMock.Setup(x => x.Now).Returns(MockDate);
            services.AddScoped(_ => dateMock.Object);

            // Replace WeatherService
            var weatherDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IWeatherService));
            if (weatherDescriptor != null)
                services.Remove(weatherDescriptor);

            services.AddScoped(_ => CreateWeatherServiceMock());
        });
    }
}
public class CoffeeTestAprilFoolsWebApplicationFactory : CoffeeTestWebApplicationFactory
{
    protected override DateTimeOffset MockDate => new(2025, 4, 1, 10, 0, 0, TimeSpan.FromHours(8));
}


public class CoffeeTestFailingWeatherFactory : CoffeeTestWebApplicationFactory
{
    protected override IWeatherService CreateWeatherServiceMock()
    {
        var mock = new Mock<IWeatherService>();
        mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Weather failed"));

        return mock.Object;
    }
}

 public class CoffeeTestHotWeatherFactory : CoffeeTestWebApplicationFactory
    {
        protected override IWeatherService CreateWeatherServiceMock()
        {
            var mock = new Mock<IWeatherService>();
            mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(35);

            return mock.Object;
        }
    }

public class CoffeeTestColdWeatherFactory : CoffeeTestWebApplicationFactory
{
    protected override IWeatherService CreateWeatherServiceMock()
    {
        var mock = new Mock<IWeatherService>();
        mock.Setup(x => x.GetCurrentTemperatureAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);

        return mock.Object;
    }
}
