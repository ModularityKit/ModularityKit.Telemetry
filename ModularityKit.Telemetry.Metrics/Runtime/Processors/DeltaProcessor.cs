using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ModularityKit.Telemetry.Metrics.Abstractions.Buffers;
using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;
namespace ModularityKit.Telemetry.Metrics.Runtime.Processors;

/// <summary>
/// A metric processor that computes the delta of counter-type metrics before forwarding them downstream.
/// </summary>
/// <remarks>
/// For each <see cref="MetricType.Counter"/> snapshot, this processor calculates the difference between
/// the current value and the previous value. Only positive deltas are emitted downstream.  
/// Non-counter metrics or negative/NaN values are passed through or skipped as appropriate.
/// </remarks>
internal sealed class DeltaProcessor(IMetricProcessor downstream, ILogger? logger = null) : IMetricProcessor
{
    private readonly ConcurrentDictionary<TimeSeriesKey, double> _previous = new();
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <summary>
    /// Emits single snapshot after computing the delta if applicable.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to process.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask EmitAsync(MetricSnapshot snapshot, CancellationToken ct = default)
    {
        if (snapshot.Metric.Type != MetricType.Counter)
            return downstream.EmitAsync(snapshot, ct);

        if (double.IsNaN(snapshot.Value) || snapshot.Value < 0)
            return ValueTask.CompletedTask;

        var key = new TimeSeriesKey(snapshot.Metric, snapshot.GetAllTags(), snapshot.Environment);

        if (!_previous.TryGetValue(key, out var prev))
        {
            _previous[key] = snapshot.Value;
            return ValueTask.CompletedTask;
        }

        double delta = snapshot.Value >= prev
            ? snapshot.Value - prev
            : snapshot.Value;

        _previous[key] = snapshot.Value;

        if (delta == 0)
            return ValueTask.CompletedTask;

        var deltaSnapshot = snapshot with { Value = delta };
        _logger.LogDebug("[DeltaProcessor] {Metric} delta = {Delta}", snapshot.Metric.Name, delta);
        return downstream.EmitAsync(deltaSnapshot, ct);
    }

    /// <summary>
    /// Emits batch of snapshots, computing deltas for counter metrics.
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
