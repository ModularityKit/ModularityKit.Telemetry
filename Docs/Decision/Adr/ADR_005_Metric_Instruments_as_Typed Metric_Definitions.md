# ADR-005: Metric Instruments as Typed Metric Definitions

## Tag
#adr_005

## Status
Accepted

## Date
2026-02-02

## Scope
ModularityKit.Telemetry.Metrics.Abstractions.Instruments

## Context

The ModularityKit telemetry system supports multiple metric types, which differ in semantics, aggregation behavior, and additional configuration parameters (eg, histogram buckets, summary quantiles).

Despite shared contract (`IMetric`) and base implementation (`MetricDefinition`), each metric type requires:

- explicit association with `MetricType`,
- preconfigured default units,
- support for type specific parameters (buckets, quantiles),
- strong semantic consistency between the definition and the runtime instrument.

Without clearly separated instrument types:

- metric definitions would be too generic,
- `MetricType` semantics would not be structurally enforced,
- the scope builder would require conditional logic,
- inconsistent or incorrect configurations could easily arise.

## Decision

### Typed Metric Definitions

For each supported metric type, a dedicated internal definition is introduced:

- `CounterMetric`
- `GaugeMetric`
- `HistogramMetric`
- `SummaryMetric`
- `TimerMetric`

Each:

- inherits from `MetricDefinition`,
- preconfigures the corresponding `MetricType`,
- encapsulates type specific parameters,
- is implemented as an immutable `record`.

### Instrument Responsibilities

#### CounterMetric

- Represents cumulative, strictly increasing metric.
- Default unit: `"1"`.

#### GaugeMetric

- Represents an instantaneous metric that varies over time.
- Default unit: `"1"`.

#### HistogramMetric

- Represents value distribution in buckets.
- Supports explicitly defined or default bucket boundaries.
- Default buckets are optimized for timing metrics.

#### SummaryMetric

- Represents statistical aggregates based on quantiles.
- Supports configurable quantiles.
- Provides sensible default quantiles (P50, P90, P95, P99).
# ADR-005: Metric Instruments as Typed Metric Definitions

￼## Tag

#adr_005

￼## Status

Accepted

￼## Date

2026-02-02

￼## Scope

ModularityKit.Telemetry.Metrics.Abstractions.Instruments

￼## Context

System telemetryczny ModularityKit wspiera wiele typów metryk, które różnią się semantyką, sposobem agregacji oraz dodatkowymi parametrami konfiguracyjnymi (np. buckety histogramu, kwantyle summary).

Pomimo wspólnego kontraktu (`IMetric`) i bazowej implementacji (`MetricDefinition`), poszczególne typy metryk wymagają:

- jawnego powiązania z `MetricType`,
    
- prekonfiguracji jednostek domyślnych,
    
- wsparcia dla typowo specyficznych parametrów (buckety, kwantyle),
- Architektura jest zgodna z zasadą **definition-first, runtime-later**.
#### TimerMetric

- Specialization of histogram for measuring durations.
- Default unit: `"ms"`.

### Visibility and Encapsulation

- All instrument definitions are `internal`.
- The public contract remains:
    - `IMetric`
    - `MetricDefinition`
    - `MetricType`
- Instrument creation occurs exclusively via:
    - `IMetricScopeBuilder`
    - the metric registry

## Design Rationale

- Strongly typed definitions eliminate semantic errors at definition time.
- `MetricType` becomes structural consequence of the definition, not just flag.
- The scope builder remains simple and declarative.
- Instruments are ready for direct mapping to runtime emitters.
- Hiding the classes (`internal`) prevents uncontrolled metric creation outside the registry.
- The architecture follows the **definition first, runtime-later** principle.