using OpenFeature.Contrib.Providers.Flagd;
using Rydet.ApiService;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddNpgsqlDataSource(connectionName: "postgresdb");

await FeatureFlagsAsync(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

app.MapGet("/weatherforecast", async () => await Weather.ForecastsAsync(summaries))
.WithName("GetWeatherForecast");

app.MapDefaultEndpoints();

await app.RunAsync();

async Task FeatureFlagsAsync(WebApplicationBuilder webApplicationBuilder)
{
    var connectionString = webApplicationBuilder.Configuration.GetConnectionString("flagd");

    await OpenFeature.Api.Instance.SetProviderAsync(
        new FlagdProvider(new Uri(connectionString!)));
}
