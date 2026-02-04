using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;

/// <summary>
/// Holds the current value and tags for <see cref="GaugeMetric"/>.
/// </summary>
/// <remarks>
/// <see cref="GaugeData"/> provides thread safe way to set and read gauge metric values.
/// Internally, the value is stored as bits of a <see cref="double"/> in a <see cref="long"/>
/// to allow atomic operations with <see cref="Interlocked"/> methods.
/// </remarks>
internal sealed class GaugeData(GaugeMetric metric, string environment, IReadOnlyDictionary<string, object?> tags)
{
    private long _valueBits;

    /// <summary>
    /// The associated gauge metric definition.
    /// </summary>
    public GaugeMetric Metric { get; } = metric;

    /// <summary>
    /// The environment in which this metric is being recorded (eg "production", "development").
    /// </summary>
    public string Environment { get; } = environment;
    
    /// <summary>
    /// Tags associated with this gauge instance.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Tags { get; } = tags;

    /// <summary>
    /// Sets the gauge to specific value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void Set(double value) =>
        Interlocked.Exchange(ref _valueBits, BitConverter.DoubleToInt64Bits(value));

    /// <summary>
    /// Gets the current value of the gauge.
    /// </summary>
    public double Value => BitConverter.Int64BitsToDouble(Interlocked.Read(ref _valueBits));
}