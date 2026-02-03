using ModularityKit.Telemetry.Metrics;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Configurations;
using ModularityKit.Telemetry.Metrics.Abstractions.Scope;
using ModularityKit.Telemetry.Metrics.Runtime;
using ModularityKit.Telemetry.Metrics.Runtime.Enrichers;
using mtel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddModularityKitTelemetry()
    .WithConfiguration(new MetricsConfiguration
    {
        Environment = "production",
        MetricBuffer = new MetricBufferConfiguration
        {
            EnableBuffering = true,
            MaxBatchSize = 100,
            MaxQueueSize = 10_000,
            FlushInterval = TimeSpan.FromSeconds(5),
            OverflowStrategy = BufferOverflowStrategy.DropOldest,
            FlushOnDispose = true
        }
    })
    .AddEnrichers(new LatencyEnricher(), new ServiceEnricher(), new EnvironmentEnricher())
    .AddFilters(new ScopeFilter(["http"]))
    .Build();


var app = builder.Build();
var registry = app.Services.GetRequiredService<MetricRegistry>();

var httpScope = new MetricScope("http", "myapp.http", "platform-team");
registry.RegisterScope(httpScope, scope =>
{
    scope
        .Counter("http_requests_total", "Total HTTP requests")
        .Histogram("http_request_duration_ms", "HTTP request duration", buckets: [50.0, 100.0, 200.0, 500.0, 1000.0]);
});

// Middleware telemetry
app.UseMetrics();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}