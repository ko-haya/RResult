namespace RResult.Api;

using Microsoft.AspNetCore.Http.HttpResults;

public readonly record struct WeatherHandler
{
    public static readonly string[] Summaries =
         ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public static Ok<WeatherForecast> CalcWeather()
    {
        return TypedResults.Ok(new WeatherForecast(
            DateTime.Now.AddDays(2),
            Random.Shared.Next(-20, 55),
            Summaries[Random.Shared.Next(Summaries.Length)]
        ));
    }
}

public readonly record struct WeatherForecast(DateTime date, int temperatureC, string? summary)
{
    public int temperatureF => 32 + (int)(temperatureC / 0.5556);
}
