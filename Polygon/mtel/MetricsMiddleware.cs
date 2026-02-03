using ModularityKit.Telemetry.Metrics.Runtime;

namespace mtel;

public class MetricsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, MetricRegistry registry, MetricsService service)
    {
        var path = context.Request.Path.ToString();
        var method = context.Request.Method;

        var counter = registry.Get("http_requests_total");
        var duration = registry.Get("http_request_duration_ms");

        using (service.Time(duration, new Dictionary<string, object?>
               {
                   ["method"] = method,
                   ["endpoint"] = path
               }))
        {
            await next(context);
        }

        await service.IncrementAsync(counter, new Dictionary<string, object?>
        {
            ["method"] = method,
            ["endpoint"] = path,
            ["status_code"] = context.Response.StatusCode
        });
    }
}

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
    {
        return app.UseMiddleware<MetricsMiddleware>();
    }
}