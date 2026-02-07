using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;
using ModularityKit.Telemetry.Metrics.Runtime.Emits;

namespace ModularityKit.Telemetry.Metrics.Runtime.Processors;

/// <summary>
/// A metric processor that deduplicates snapshots by skipping repeated values for the same time series.
/// </summary>
/// <remarks>
/// This processor keeps track of the last emitted value per <see cref="TimeSeriesKey"/>.
/// If a new snapshot has the same value (within a small epsilon) as the last one, it is skipped.
/// Otherwise, it is forwarded to the downstream processor.
/// </remarks>
internal sealed class DeduplicationProcessor(IMetricProcessor downstream, ILogger? logger = null) : IMetricProcessor
{
    private readonly ConcurrentDictionary<TimeSeriesKey, double> _lastValues = new();
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    private const double Epsilon = 1e-9;

    /// <summary>
    /// Emits single snapshot if it is not a duplicate.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to emit.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask EmitAsync(MetricSnapshot snapshot, CancellationToken ct = default)
    {
        var key = new TimeSeriesKey(snapshot.Metric, snapshot.GetAllTags(), snapshot.Environment);

        if (_lastValues.TryGetValue(key, out var last) && Math.Abs(last - snapshot.Value) < Epsilon)
        {
            _logger.LogDebug("[DeduplicationProcessor] Skipping duplicate {Metric} ≈ {Value}", snapshot.Metric.Name, snapshot.Value);
            return ValueTask.CompletedTask;
        }

        _lastValues[key] = snapshot.Value;
        return downstream.EmitAsync(snapshot, ct);
    }
    
    /// <summary>
    /// Emits batch of snapshots, deduplicating each one.
    /// </summary>
    /// <param name="snapshots">The collection of metric snapshots to process.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public async ValueTask EmitBatchAsync(IEnumerable<MetricSnapshot> snapshots, CancellationToken ct = default)
    {
        foreach (var snapshot in snapshots)
            await EmitAsync(snapshot, ct);
    }
}