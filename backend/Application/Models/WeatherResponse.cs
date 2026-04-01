namespace Application.Models;    
public class WeatherResponse
{
    public MainData Main { get; set; } = new();
}

public class MainData
{
    public double Temp { get; set; }
}