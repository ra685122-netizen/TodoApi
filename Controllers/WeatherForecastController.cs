using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class WeatherForecastController : ControllerBase
{
    private static WeatherForecast[] weatherForecasts = [];

    [HttpGet]
    public ActionResult<WeatherForecast[]> Get()
    {
        return Ok(weatherForecasts);
    }

    [HttpGet("{id}")]
    public ActionResult<WeatherForecast> GetById(int id)
    {
        
        var weather = weatherForecasts.FirstOrDefault(w => w.Id == id);

        if (weather == null)
        {
            return NotFound();
        }

        return Ok(weather);
    }
    [HttpPost]
    public ActionResult<WeatherForecast> Create(WeatherForecast weather)
    {
        if (weather == null)
        {
            return BadRequest("Weather forecast is required.");
        }

        if (string.IsNullOrWhiteSpace(weather.Summary))
        {
            return BadRequest("Summary is required.");
        }

        int newId = weatherForecasts.Length == 0
            ? 1
            : weatherForecasts.Max(w => w.Id) + 1;
        weather.Id = newId;

        weatherForecasts = [.. weatherForecasts, weather];

        return CreatedAtAction(nameof(GetById), new { id = weather.Id }, weather);
    }
    [HttpPut("{id}")]
    public ActionResult<WeatherForecast> Update(int id, WeatherForecast weather)
    {
        if (weather == null)
        {
            return BadRequest("Weather forecast is required.");
        }

        if (string.IsNullOrWhiteSpace(weather.Summary))
        {
            return BadRequest("Summary is required.");
        }

        var existingWeather = weatherForecasts.FirstOrDefault(w => w.Id == id);

        if (existingWeather == null)
        {
            return NotFound();
        }

        existingWeather.Date = weather.Date;
        existingWeather.TemperatureC = weather.TemperatureC;
        existingWeather.Summary = weather.Summary;

        return Ok(existingWeather);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var index = Array.FindIndex(weatherForecasts, w => w.Id == id);

        if (index == -1)
        {
            return NotFound();
        }

        weatherForecasts = weatherForecasts
            .Where(w => w.Id != id)
            .ToArray();

        return NoContent();
    }
}