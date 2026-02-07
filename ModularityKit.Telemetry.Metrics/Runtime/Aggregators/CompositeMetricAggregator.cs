using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators;

/// <summary>
/// Routes metric snapshots to the appropriate aggregator based on the metric type.
/// </summary>
/// <remarks>
/// <see cref="CompositeMetricAggregator"/> acts as a facade over individual aggregators such as
/// <see cref="GaugeAggregator"/>, <see cref="CounterAggregator"/>, and <see cref="HistogramAggregator"/>.
/// It forwards snapshots to the correct aggregator and provides a unified <see cref="FlushAsync"/> method
/// to flush all aggregated metrics.
/// </remarks>
internal sealed class CompositeMetricAggregator : IMetricAggregator
{
    private readonly IReadOnlyDictionary<Type, IMetricAggregator> _aggregators;
    private readonly IEnumerable<IMetricAggregator> _allAggregators;

    /// <summary>
    /// Initializes new instance of <see cref="CompositeMetricAggregator"/> with specific aggregators.
    /// </summary>
    /// <param name="gaugeAggregator">Aggregator for <see cref="GaugeMetric"/> snapshots.</param>
    /// <param name="counterAggregator">Aggregator for <see cref="CounterMetric"/> snapshots.</param>
    /// <param name="histogramAggregator">Aggregator for <see cref="HistogramMetric"/> snapshots.</param>
    public CompositeMetricAggregator(
        GaugeAggregator gaugeAggregator,
        CounterAggregator counterAggregator,
        HistogramAggregator histogramAggregator)
    {
        ArgumentNullException.ThrowIfNull(gaugeAggregator);
        ArgumentNullException.ThrowIfNull(counterAggregator);
        ArgumentNullException.ThrowIfNull(histogramAggregator);

        _aggregators = new Dictionary<Type, IMetricAggregator>
        {
            [typeof(GaugeMetric)] = gaugeAggregator,
            [typeof(CounterMetric)] = counterAggregator,
            [typeof(HistogramMetric)] = histogramAggregator
        };

        _allAggregators = _aggregators.Values;
    }

    /// <summary>
    /// Adds <see cref="MetricSnapshot"/> to appropriate aggregator based on its metric type.
    /// </summary>
    /// <param name="snapshot">The metric snapshot to add.</param>
    public void Add(MetricSnapshot snapshot)
    {
        var metricType = snapshot.Metric.GetType();
        
        if (_aggregators.TryGetValue(metricType, out var aggregator))
        {
            aggregator.Add(snapshot);
        }
    }

    /// <summary>
    /// Flushes all underlying aggregators and returns all emitted <see cref="MetricSnapshot"/> instances.
    /// </summary>
    /// <returns>A collection of flushed metric snapshots.</returns>
    public async Task<IEnumerable<MetricSnapshot>> FlushAsync()
    {
        var results = await Task.WhenAll(_allAggregators.Select(a => a.FlushAsync()));
        return results.SelectMany(x => x);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var aggregator in _allAggregators.OfType<IAsyncDisposable>())
            await aggregator.DisposeAsync();
    }
}
