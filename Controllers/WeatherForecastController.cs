using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet("all")]
    public string GetAll()
    {
        return "This is GET ALL";
    }

    [HttpGet("today")]
    public string GetToday()
    {
        return "This is GET TODAY";
    }

    [HttpGet("{id}")]
    public string GetById(int id)
    {
        return $"This is GET BY ID: {id}";
    }

    [HttpPost]
    public WeatherForecast Create(WeatherForecast weather)
    {
        return weather;
    }
    [HttpPut("{id}")]
    public string Update(int id, WeatherForecast weather)
    {
    return $"Updated ID: {id}, New Summary: {weather.Summary}";
    }
    [HttpDelete("{id}")]
    public string Delete(int id)
    {
    return $"Deleted ID: {id}";
    }
}
