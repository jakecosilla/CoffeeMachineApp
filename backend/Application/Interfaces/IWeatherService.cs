namespace Application.Interfaces;
public interface IWeatherService
{
    Task<double> GetCurrentTemperatureAsync(CancellationToken cancellationToken);
}