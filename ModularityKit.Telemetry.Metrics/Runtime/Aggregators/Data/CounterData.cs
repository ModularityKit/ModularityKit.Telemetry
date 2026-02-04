using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;

/// <summary>
/// Holds the current value and tags for <see cref="CounterMetric"/>.
/// </summary>
/// <remarks>
/// <see cref="CounterData"/> provides thread safe way to increment counter metrics.
/// Internally, the value is stored as bits of a <see cref="double"/> in a <see cref="long"/>
/// to allow atomic operations with <see cref="Interlocked"/> methods.
/// </remarks>
internal sealed class CounterData(CounterMetric metric, string environment, IReadOnlyDictionary<string, object?> tags)
{
    private long _valueBits;

    /// <summary>
    /// The associated counter metric definition.
    /// </summary>
    public CounterMetric Metric { get; } = metric;
    
    /// <summary>
    /// The environment in which this metric is being recorded (eg "production", "development").
    /// </summary>
    public string Environment { get; } = environment;

    /// <summary>
    /// Tags associated with this counter instance.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Tags { get; } = tags;

    /// <summary>
    /// Increments the counter by a specified value.
    /// </summary>
    /// <param name="value">The amount to increment. Must be non-negative.</param>
    /// <exception cref="InvalidOperationException">Thrown if a negative value is provided.</exception>
    public void Increment(double value)
    {
        if (value < 0) throw new InvalidOperationException("Counter cannot be decremented");

        long oldBits, newBits;
        do
        {
            oldBits = Interlocked.Read(ref _valueBits);
            var oldVal = BitConverter.Int64BitsToDouble(oldBits);
            var newVal = oldVal + value;
            newBits = BitConverter.DoubleToInt64Bits(newVal);
        } while (Interlocked.CompareExchange(ref _valueBits, newBits, oldBits) != oldBits);
    }

    /// <summary>
    /// Gets the current value of the counter.
    /// </summary>
    public double Value => BitConverter.Int64BitsToDouble(Interlocked.Read(ref _valueBits));
}