using Microsoft.Extensions.Logging;

namespace ModularityKit.Telemetry.Metrics.Abstractions;

public static class MetricLogEvents
{
    public static readonly EventId BufferFlow = new(101, "MetricsBuffer");
    public static readonly EventId PipelineAudit = new(102, "MetricsAudit");
    public static readonly EventId CollectorError = new(103, "CollectorError");
}