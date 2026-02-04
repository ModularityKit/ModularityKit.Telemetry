using ModularityKit.Telemetry.Metrics.Abstractions.Instruments;
using ModularityKit.Telemetry.Metrics.Abstractions.Scope;

namespace ModularityKit.Telemetry.Metrics.Runtime.Aggregators.Data;

/// <summary>
/// Holds histogram data including bucket counts, sum, and associated tags.
/// </summary>
/// <remarks>
/// <see cref="HistogramData"/> is used to record values into defined buckets for a histogram metric.
/// The sum of all recorded values is maintained using atomic operations for thread safety.
/// Buckets are defined as thresholds, with an extra bucket representing +Infinity.
/// </remarks>
internal sealed class HistogramData(HistogramMetric metric, string environment, IReadOnlyDictionary<string, object?> tags)
{
    /// <summary>
    /// The histogram bucket thresholds.
    /// </summary>
    public readonly double[] Buckets = metric.Buckets;

    /// <summary>
    /// Count of values in each bucket. Last element represents +Infinity.
    /// </summary>
    public readonly long[] Counts = new long[metric.Buckets.Length + 1];

    private long _sumBits;

    /// <summary>
    /// The metric scope this histogram belongs to.
    /// </summary>
    public readonly MetricScope? Scope = metric.Scope;

    /// <summary>
    /// The environment in which this metric is being recorded (eg "production", "development").
    /// </summary>
    public string Environment { get; } = environment;
    
    /// <summary>
    /// Tags associated with this histogram instance.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Tags { get; } = tags;

    /// <summary>
    /// The name of the metric.
    /// </summary>
    public string MetricName { get; } = metric.Name;

    /// <summary>
    /// Records a value into the histogram, incrementing the appropriate bucket and updating the sum.
    /// </summary>
    /// <param name="value">The value to record.</param>
    public void Record(double value)
    {
        long oldBits, newBits;
        do
        {
            oldBits = Interlocked.Read(ref _sumBits);
            var oldVal = BitConverter.Int64BitsToDouble(oldBits);
            var newVal = oldVal + value;
            newBits = BitConverter.DoubleToInt64Bits(newVal);
        } while (Interlocked.CompareExchange(ref _sumBits, newBits, oldBits) != oldBits);
        
        for (var i = 0; i < Buckets.Length; i++)
        {
            if (!(value <= Buckets[i])) continue;
            Interlocked.Increment(ref Counts[i]);
            return;
        }
        Interlocked.Increment(ref Counts[^1]); // +Inf bucket
    }

    /// <summary>
    /// Gets the sum of all values recorded in the histogram.
    /// </summary>
    public double Sum => BitConverter.Int64BitsToDouble(Interlocked.Read(ref _sumBits));
}