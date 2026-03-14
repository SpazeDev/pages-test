namespace PagesTest.LocalHelper;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors();

        var app = builder.Build();
        app.UseCors(x =>
        {
            x.AllowAnyHeader();
            x.AllowAnyMethod();
            x.AllowAnyOrigin();
        });

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/test", (HttpContext httpContext) =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
                .ToArray();
            return forecast;
        });

        app.MapGet("/sse", async (CancellationToken ct) =>
        {
            async IAsyncEnumerable<string> GetData()
            {
                while (!ct.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
                    yield return Random.Shared.Next(0, 100).ToString();
                }
            }

            return TypedResults.ServerSentEvents(GetData());
        });

        app.Run();
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}