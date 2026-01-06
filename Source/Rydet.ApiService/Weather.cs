using System.Diagnostics.CodeAnalysis;

namespace Rydet.ApiService;

[SuppressMessage("Design", "MA0048:File name must match type name")]
public sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string la)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public static class Weather
{

    public static async Task<WeatherForecast[]> ForecastsAsync(string[] strings)
    {
        var flagClient = OpenFeature.Api.Instance.GetClient();
        var forecast = await Task.WhenAll(Enumerable.Range(1, 5).Select(async index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
#pragma warning disable CA5394
                Random.Shared.Next(-20, 55),
                strings[Random.Shared.Next(strings.Length)],
#pragma warning restore CA5394

                await flagClient.GetStringValueAsync(
                    "background-color",
                    defaultValue: "#000000")
            )));
        return forecast;
    }
}
