# ADR-010: Metric Buffers and Overflow Strategies

## Tag
#adr_010

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Buffers

## Context

In telemetry systems, metrics can be emitted faster than they are processed or sent.  
Without dedicated buffering:

- excess snapshots would be lost or could block the system,
- emitters and collectors would need to handle overload manually,
- lack of control over full buffer behavior could lead to data inconsistencies and system instability.

Different scenarios require different overflow strategies:

- in high-throughput systems, preserving the latest data is critical,
- in some cases, preserving all historical values is prioritized, even at the cost of blocking the producer.

## Decision

### Metric Buffers (`IMetricBuffer`)

- Temporarily stores metric snapshots before processing or emission.
- Decouples metric producers from consumers (collectors/emitters), absorbing sudden spikes in metric volume.
- Supports asynchronous insertion of snapshots (`EnqueueAsync`) and cancellation without flushing (`CancelWithoutFlushAsync`).
- Implementations may apply batching, backpressure, and overflow strategies depending on scenario requirements.

### Buffer Overflow Strategies (`BufferOverflowStrategy`)

- `DropOldest` – removes the oldest snapshots, prioritizing fresh data; suitable for real-time systems.
- `DropNewest` – discards newly added snapshots, preserving already stored data; useful when historical data integrity matters.
- `BlockProducer` – blocks the producer until space becomes available; eliminates data loss but introduces potential latency.

### Internal Lifecycle (`MetricBufferState`)

- `Running` – buffer actively accepts and processes snapshots.
- `Draining` – buffer stops accepting new snapshots and processes the remaining ones.
- `Stopped` – buffer has completed processing and released resources.

## Design Rationale

- Buffers protect the telemetry system from overload and data loss.
- Overflow strategies allow tailoring buffer behavior to business requirements and SLA.
- Lifecycle states enable safe shutdown and controlled flushing of snapshots.
- Asynchronous APIs improve scalability and integration with high-throughput emitters and pipelines.
- Separating buffer logic from collectors/emitters maintains clean separation of responsibilities.