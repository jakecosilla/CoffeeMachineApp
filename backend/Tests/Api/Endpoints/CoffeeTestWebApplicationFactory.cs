using Application;
using Application.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Tests.Api.Endpoints
{
    public class CoffeeTestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected virtual DateTime MockDate => new DateTime(2025, 1, 15, 10, 0, 0);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the default IDateTimeProvider registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDateTimeProvider));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add a mocked IDateTimeProvider for testing with the specified date
                var dateTimeProviderMock = new Mock<IDateTimeProvider>();
                dateTimeProviderMock.Setup(x => x.Now).Returns(MockDate);
                services.AddScoped<IDateTimeProvider>(_ => dateTimeProviderMock.Object);
            });
        }
    }

    public class CoffeeTestAprilFoolsWebApplicationFactory : CoffeeTestWebApplicationFactory
    {
        protected override DateTime MockDate => new DateTime(2025, 4, 1, 10, 0, 0);
    }
}
