# ADR-011: Metrics Configuration and Limits

## Tag
#adr_011

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Configurations

## Context

The telemetry system requires centralized configuration for all aspects of metric emission to ensure:

- control over the environment (`Environment`),
- system safety under high tag volume (cardinality),
- stability and performance via buffering and overflow strategies,
- throttling of metric throughput to backends via rate limiting.

Without clear and consistent configuration:

- pipelines and backends could be overloaded,
- uncontrolled growth of unique tag combinations could occur,
- lack of batching or aggregation would lead to high emission overhead,
- predictable behavior of the telemetry system would be harder to maintain.

## Decision

### MetricsConfiguration

- Aggregates all telemetry settings in one place: buffering, cardinality, aggregation, rate limiting.
- Can be associated with environment logic (`Environment`) and global tags.
- Designed for binding to application configuration.

### CardinalityLimitConfiguration

- Protects against excessive unique tag combinations, which could exhaust memory or overload backends.
- Parameters:
    - `Enabled` – enables/disables cardinality limiting.
    - `MaxCardinality` – maximum number of unique tag combinations.
    - `ResetInterval` – interval after which the unique combination counter resets.

### MetricBufferConfiguration

- Buffers metrics to allow batching and throughput control.
- Parameters:
    - `EnableBuffering` – enables/disables buffering.
    - `MaxBatchSize` – maximum batch size.
    - `MaxQueueSize` – maximum number of snapshots in the queue.
    - `OverflowStrategy` – overflow behavior (`DropOldest`, `DropNewest`, `BlockProducer`).
    - `FlushInterval` – automatic flush interval.
    - `FlushOnDispose` – flush buffer on disposal.

### RateLimitConfiguration

- Limits the throughput of metric emission to backends.
- Parameters:
    - `Enabled` – enables/disables rate limiting.
    - `MaxMetricsPerSecond` – maximum metrics per second.
    - `BurstSize` – maximum metrics in a short spike.

### EnableAggregation

- Allows aggregating multiple samples before emission, reducing I/O operations at the cost of temporal resolution.

## Design Rationale

- Centralized configuration enables consistent management of all aspects of metric emission.
- Cardinality and rate limiting protect the system and backends from overload.
- Buffering and flush strategies ensure performance and predictable behavior under load.
- Aggregation provides balance between accuracy and efficiency.
- The configuration is designed to be easily extendable for future policies and limits.