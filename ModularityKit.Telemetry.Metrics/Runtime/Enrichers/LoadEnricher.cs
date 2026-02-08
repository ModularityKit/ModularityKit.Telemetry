using System.Diagnostics;
using System.Runtime.InteropServices;
using ModularityKit.Telemetry.Metrics.Abstractions.Policy;
using ModularityKit.Telemetry.Metrics.Abstractions.Snapshots;

namespace ModularityKit.Telemetry.Metrics.Runtime.Enrichers;

/// <summary>
/// Enriches metric snapshots with process and system resource usage metrics.
/// </summary>
/// <remarks>
/// Provides detailed runtime diagnostics including CPU, memory, thread count, uptime,
/// and optionally system load on Unix-based systems.
/// Designed for correlating metrics with process and system health.
/// </remarks>
public sealed class LoadEnricher : IMetricEnricher
{
    private readonly Process _currentProcess = Process.GetCurrentProcess();
    private readonly DateTime _startTime = DateTime.UtcNow;

    /// <summary>
    /// Enriches a metric snapshot with runtime process and system load statistics.
    /// </summary>
    /// <param name="snapshot">The snapshot to enrich.</param>
    /// <returns>A new <see cref="MetricSnapshot"/> instance containing the enriched tags.</returns>
    public MetricSnapshot Enrich(MetricSnapshot snapshot)
    {
        var processCpu = GetProcessCpuUsage();
        var processMemory = _currentProcess.WorkingSet64;
        var privateMemory = _currentProcess.PrivateMemorySize64;
        var virtualMemory = _currentProcess.VirtualMemorySize64;
        var gcMemory = GC.GetTotalMemory(false);
        var threadCount = _currentProcess.Threads.Count;
        var uptimeMs = (DateTime.UtcNow - _currentProcess.StartTime.ToUniversalTime()).TotalMilliseconds;
        var systemLoad = GetSystemLoad();

        var enrichedTags = new Dictionary<string, object?>(snapshot.Tags)
        {
            ["process_cpu_percent"] = processCpu,
            ["process_memory_bytes"] = processMemory,
            ["private_memory_bytes"] = privateMemory,
            ["virtual_memory_bytes"] = virtualMemory,
            ["gc_memory_bytes"] = gcMemory,
            ["thread_count"] = threadCount,
            ["uptime_ms"] = uptimeMs,
            ["system_load_average"] = systemLoad
        };

        return snapshot with { Tags = enrichedTags };
    }

    /// <summary>
    /// Calculates the approximate CPU usage percentage for the current process.
    /// </summary>
    /// <returns>CPU usage as a percentage (0-100), rounded to 2 decimals.</returns>
    private double GetProcessCpuUsage()
    {
        var totalMs = _currentProcess.TotalProcessorTime.TotalMilliseconds;
        var uptimeMs = (DateTime.UtcNow - _currentProcess.StartTime.ToUniversalTime()).TotalMilliseconds;
        var cpuPercent = totalMs / uptimeMs / Environment.ProcessorCount * 100;
        return Math.Round(cpuPercent, 2);
    }

    /// <summary>
    /// Retrieves the system load average on Unix-based systems (Linux/macOS).
    /// </summary>
    /// <returns>
    /// System load as a double, rounded to 2 decimals, or <c>null</c> on unsupported platforms or on failure.
    /// </returns>
    private double? GetSystemLoad()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            !RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return null;

        try
        {
            var loadAvg = File.ReadAllText("/proc/loadavg")
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

            if (double.TryParse(loadAvg, out var load))
                return Math.Round(load, 2);
        }
        catch
        {
            // ignored
        }

        return null;
    }
}