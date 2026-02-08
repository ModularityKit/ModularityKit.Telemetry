using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Enrichers;

/// <summary>
/// Adds processing latency information to metric snapshots.
/// </summary>
/// <remarks>
/// The latency is measured relative to the lifetime of this enricher instance,
/// not the actual operation that produced the metric.
/// <para>
/// This means the value represents:
/// <code>
/// now - enricher_creation_time
/// </code>
/// rather than request execution time.
/// </para>
/// <para>
/// Because of this behavior, the enricher should typically be scoped per request,
/// operation, or pipeline execution. When registered as a singleton it will produce
/// continuously increasing latency values.
/// </para>
/// <para>
/// Added tags:
/// <list type="bullet">
/// <item><c>request_start</c> — time when the enricher instance was created</item>
/// <item><c>request_end</c> — enrichment time</item>
/// <item><c>latency_ms</c> — elapsed milliseconds between start and enrichment</item>
/// </list>
/// </para>
/// <para>
/// A new tag dictionary is allocated per snapshot to preserve snapshot immutability.
/// </para>
/// </remarks>
public sealed class LatencyEnricher : IMetricEnricher
{
    private readonly DateTime _startTime = DateTime.UtcNow;

    /// <summary>
    /// Enriches the snapshot with latency timing information.
    /// </summary>
    /// <param name="snapshot">The snapshot to enrich.</param>
    /// <returns>A new <see cref="MetricSnapshot"/> containing latency tags.</returns>
    public MetricSnapshot Enrich(MetricSnapshot snapshot)
    {
        var endTime = DateTime.UtcNow;
        var durationMs = (endTime - _startTime).TotalMilliseconds;

        var enriched = new Dictionary<string, object?>(snapshot.Tags)
        {
            ["request_start"] = _startTime,
            ["request_end"] = endTime,
            ["latency_ms"] = Math.Round(durationMs, 2)
        };

        return snapshot with { Tags = enriched };
    }
}
