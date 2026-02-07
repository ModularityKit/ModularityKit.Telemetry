using ModularityKit.Telemetry.Metrics.Abstractions.Emits;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Collectors;

public sealed class ConsoleMetricCollector : IMetricCollector
{
    public void Record(MetricSnapshot snapshot)
    {
        var tags = FormatTags(snapshot.GetAllTags());
        var output = $"[{snapshot.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] " +
                     $"{snapshot.Metric.Type} | {snapshot.FullName} = {snapshot.Value:F2} {snapshot.Metric.Unit}" +
                     (string.IsNullOrEmpty(tags) ? "" : $" | {tags}");

        Console.WriteLine(output);
    }

    public Task RecordAsync(MetricSnapshot snapshot, CancellationToken ct = default)
    {
        Record(snapshot);
        return Task.CompletedTask;
    }
    
    public Task RecordBatchAsync(ReadOnlyMemory<MetricSnapshot> snapshots, CancellationToken ct = default)
    {
        var span = snapshots.Span;
        foreach (var snapshot in span)
        {
            Record(snapshot);
        }
        return Task.CompletedTask;
    }
    
    private static string FormatTags(IReadOnlyDictionary<string, object?> tags) =>
        tags.Count == 0
            ? string.Empty
            : string.Join(", ", tags.Select(t => $"{t.Key}={t.Value}"));
}