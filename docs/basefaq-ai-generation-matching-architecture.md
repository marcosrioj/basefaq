# BaseFAQ AI Architecture Proposal

## Executive Summary
This document defines how `BaseFaq.Faq.AI.Generation` and `BaseFaq.Faq.AI.Matching` should be added to BaseFAQ with minimal incremental changes.

The proposal preserves the existing architecture model:
- Existing API hosts remain composition roots.
- Existing `Business` modules continue using MediatR orchestration.
- Existing `FaqDbContext` remains the persistence boundary for FAQ domain data.
- Existing infrastructure conventions (tenant resolution, auth, Sentry, Redis, MassTransit package baseline) remain in place.

The new AI capability is added under a dedicated root solution folder:
- `BaseFaq.Faq.AI/Generation`
- `BaseFaq.Faq.AI/Matching`
- `BaseFaq.Faq.AI/Common`

## Recommended Architecture (Aligned to Existing BaseFAQ Model)
### Architectural assumptions
- No active production-grade queue consumers are currently implemented.
- AI generation and matching are part of the FAQ bounded context.
- Existing folder conventions (`Api`, `Business`, `Test`) should be preserved.
- Existing tenant model remains authoritative (`TenantDb` + `X-Tenant-Id` or `X-Client-Key` resolution paths).

### Existing model to preserve
- API startup and middleware pattern in API host projects.
- Feature composition via extension methods in `Api/Extensions`.
- Application orchestration in Business projects with MediatR handlers.
- Shared persistence in `BaseFaq.Faq.Common.Persistence.FaqDb`.
- Shared infrastructure utilities from `BaseFaq.Common.Infrastructure.*`.

### Target high-level model
- `Generation`: asynchronous workflow driven by events and worker consumers.
- `Matching`: synchronous query path for end-user retrieval, with asynchronous background indexing/embedding refresh.
- `Common`: provider and vector abstractions shared by both modules.

## Current Solution Style + Incremental Additions
### Existing BaseFAQ components (unchanged)
- `BaseFaq.Faq.Portal.Api` and `BaseFaq.Faq.Public.Api` startup conventions.
- Existing middleware: auth, tenant resolution/client key resolution, API error handling, Sentry.
- Existing `BaseFaq.Faq.Portal.Business.*` and `BaseFaq.Faq.Public.Business.*` patterns.
- Existing entity model and DB context ownership in `BaseFaq.Faq.Common.Persistence.FaqDb`.

### Existing components with additive changes only
- `BaseFaq.Faq.Portal.Api`:
  - Register AI generation feature in existing `AddFeatures(...)`.
  - Add generation endpoints through new Business module controllers.
- `BaseFaq.Faq.Public.Api`:
  - Register AI matching feature in existing `AddFeatures(...)`.
  - Add matching endpoints through new Business module controllers.
- `BaseFaq.Faq.Common.Persistence.FaqDb`:
  - Add AI-specific entities and mappings for lifecycle tracking and versioning.
  - Add migrations using existing migration tooling.

### New components (added)
- New AI projects under `BaseFaq.Faq.AI` only (no restructuring of existing modules).
- New worker processes for asynchronous generation and optional embedding refresh.
- New contracts shared for AI events and provider abstractions.

## BaseFaq.Faq.AI.Generation and BaseFaq.Faq.AI.Matching Project Divisions
| Division | Responsibility | Recommended .NET technologies/libraries | Applicable patterns | Risks | Mitigations |
|---|---|---|---|---|---|
| API integration endpoints | Expose commands/queries in existing API model (`Portal` for generation, `Public` for matching) | ASP.NET Core controllers, existing auth/tenant middleware | Thin controller + application service | API bloat | Keep strict route namespace `api/faqs/ai/*` |
| Application orchestration layer | Validate input, coordinate workflows, dispatch commands/queries | MediatR, FluentValidation (optional) | CQRS, orchestration service | Business logic leakage into controllers | Keep orchestration in handlers/services only |
| AI processing worker/service | Consume generation jobs, call LLM provider, persist outputs, emit completion/failure events | Worker Service, MassTransit consumer, Polly, HttpClientFactory | Async consumer, retry/circuit-breaker | No HTTP context in workers | Include tenant/user metadata in messages + worker context service |
| Domain rules for FAQ generation lifecycle | Enforce status transitions and review gates | Domain services + enums + guard methods | State machine | Invalid transitions | Transition guard + integration tests |
| Persistence and FAQ versioning | Store jobs, versions, generated artifacts, quality score, lineage | EF Core, Npgsql, existing migration flow | Append-only version history + current pointer | Data growth | Retention policy + archival strategy |
| Messaging/events integration | Decouple request/processing/completion and status notifications | MassTransit + RabbitMQ | Event-driven architecture, outbox/inbox | Duplicate delivery | Idempotency keys + processed-message store |
| Security and secret management | Secure provider keys and prevent secret leakage | .NET config providers, User Secrets (dev), cloud secret manager | Secret abstraction + rotation | Secret exposure | Vault-only in non-dev + redaction filters |
| Observability and operations | Visibility across APIs, workers, broker, DB, provider calls | Sentry (existing), OpenTelemetry, HealthChecks, structured logs | Correlation tracing + SLO monitoring | Blind failure modes | End-to-end tracing + alerting thresholds |
| Prompt governance and answer quality controls | Prompt versioning, policy checks, publication gates | Prompt registry (JSON/YAML + DB ref), evaluation runner (optional) | Prompt-as-code + human-in-the-loop | Hallucinations/quality drift | Quality rubric + approval workflow + fallback |

## Event Flow (Step-by-Step)
### End-to-end
1. Client calls `POST /api/faqs/ai/generation-jobs` (Portal API).
2. API validates tenant/user context and stores `GenerationJob` as `Requested`.
3. API publishes `FaqGenerationRequestedV1` with correlation and idempotency metadata.
4. AI Generation worker consumes request event.
5. Worker updates job to `Processing`.
6. Worker loads prompt profile and source context.
7. Worker calls provider (`OpenAI`/`Azure OpenAI`) with retries and timeout policy.
8. Worker applies quality checks and business rules.
9. Worker persists generated version/artifacts in FAQ DB.
10. Worker sets final status:
   - `AwaitingReview` (human approval required), or
   - `Completed` / `Published`, or
   - `Failed`.
11. Worker publishes final event (`Completed` or `Failed`).
12. API status endpoint (`GET /api/faqs/ai/generation-jobs/{id}`) reflects final state.

## Synchronous vs Asynchronous Integration
### Use synchronous when
- The caller needs immediate answer and bounded latency is acceptable.
- Example: FAQ matching query in Public API (`search/match` path).
- Timeout budget should be strict; fallback to lexical match if vector/provider unavailable.

### Use asynchronous when
- Work is compute-heavy, long-running, expensive, or failure-prone.
- Example: FAQ generation, re-generation, bulk embedding refresh.
- API should return `202 Accepted` with job identifier and polling/subscription mechanism.

### Rule of thumb
- `Generation`: async by default.
- `Matching`: sync query path + async index maintenance.

## RabbitMQ and MassTransit Evaluation
### Scenario A: Use both together (recommended)
- Use RabbitMQ as transport broker.
- Use MassTransit as .NET messaging abstraction/runtime.

Pros:
- Faster delivery for consumers/producers.
- Built-in middleware for retry, delayed retry, error queues, topology.
- Better developer ergonomics and observability hooks.

Cons:
- Additional abstraction layer to operate and understand.

### Scenario B: Use only RabbitMQ client library
Pros:
- Lower abstraction overhead.
- Full control of broker semantics.

Cons:
- More boilerplate for retries, correlation, poison handling, and instrumentation.
- Higher maintenance and inconsistency risk across services.

### Scenario C: Use only MassTransit (without RabbitMQ)
- Not aligned to current base services if RabbitMQ is the broker baseline.
- Requires selecting another transport and operating model.

### Trade-offs
| Option | Complexity | Resilience | Observability | Operational cost |
|---|---|---|---|---|
| RabbitMQ + MassTransit | Medium | High | High | Medium |
| RabbitMQ only | Medium-High (app code) | Medium | Medium-Low | Medium |
| Alternative transport with MassTransit only | Medium-High (migration) | Medium-High | Medium-High | Medium-High |

Recommended for BaseFAQ:
- Keep RabbitMQ as broker.
- Standardize AI messaging on MassTransit abstractions.

## Idempotency, Retry, DLQ, Tracing, Monitoring
### Idempotency strategy
- Require `Idempotency-Key` on generation command API.
- Persist unique key at job creation (`TenantId + IdempotencyKey` unique index).
- Consumer dedupe:
  - Store processed `MessageId` + handler name.
  - Skip if already processed.

### Retry policy
- Transport/consumer-level retries for transient infrastructure issues.
- Provider-level retries with exponential backoff + jitter (429/5xx/timeouts).
- No retries for validation/domain errors.

### Dead-letter queue strategy
- Route poison messages to `_error` queues.
- Add AI-specific quarantine queue for messages exceeding retry policy.
- Operational runbook:
  - Inspect cause.
  - Patch/redeploy.
  - Replay safe messages with dedupe check.

### Distributed tracing
- Propagate `traceparent`, `CorrelationId`, `CausationId` through API -> broker -> worker -> DB/provider.
- Add spans for:
  - API command handling.
  - Event publish/consume.
  - Provider API call.
  - Persistence operations.

### Monitoring and alerting
- Metrics:
  - Job throughput and completion rate.
  - Failure rate by error code.
  - Queue depth and message age.
  - p95 and p99 generation duration.
  - Matching latency.
- Alerts:
  - Queue lag threshold exceeded.
  - Failure ratio above baseline.
  - DLQ growth.
  - Provider error spikes / rate limiting spikes.

## OpenAI API Key Security Strategy
### Secret manager
- Development: .NET User Secrets.
- Production: managed secret manager (Azure Key Vault / AWS Secrets Manager / GCP Secret Manager).
- Never store provider keys in repository `appsettings*.json`.

### Key rotation
- Support active/standby key references.
- Rotate regularly (time-based) and on incident trigger.
- Use `IOptionsMonitor` or equivalent config reload for zero-downtime key switch.

### Log masking and no leakage
- Redact:
  - `Authorization` headers.
  - API keys and tokens.
  - Prompt fragments containing sensitive content.
- Disable verbose provider request/response logs in production.
- Add logging guard middleware/sinks with explicit masking rules.

## Implementation Plan by Phase
### Phase 1: MVP
- Create AI project skeleton under `BaseFaq.Faq.AI`.
- Implement generation command + async worker processing.
- Implement generation status endpoint.
- Add minimum persistence model for job + generated version.
- Implement matching endpoint with basic vector + fallback search.
- Add integration tests for happy-path generation and matching.

### Phase 2: Production Hardening
- Add idempotency enforcement and consumer dedupe store.
- Configure robust retry and DLQ policies.
- Add full tracing and alerting.
- Introduce prompt governance and quality gates.
- Enforce secret manager in non-dev environments.

### Phase 3: Scale
- Separate workers by workload type (generation vs embedding/index refresh).
- Introduce batching and adaptive concurrency controls.
- Add cost controls and provider routing strategy.
- Improve relevance quality via hybrid retrieval and re-ranking.

## Practical Artifacts
### Event contract examples
```csharp
public record FaqGenerationRequestedV1(
    Guid EventId,
    Guid CorrelationId,
    Guid TenantId,
    Guid JobId,
    Guid RequestedByUserId,
    string IdempotencyKey,
    Guid FaqId,
    string Language,
    string PromptProfile,
    DateTime OccurredUtc);

public record FaqGenerationCompletedV1(
    Guid EventId,
    Guid CorrelationId,
    Guid TenantId,
    Guid JobId,
    Guid FaqVersionId,
    bool RequiresHumanReview,
    DateTime OccurredUtc);

public record FaqGenerationFailedV1(
    Guid EventId,
    Guid CorrelationId,
    Guid TenantId,
    Guid JobId,
    string ErrorCode,
    string ErrorMessage,
    DateTime OccurredUtc);
```

### Solution/project folder structure
```text
dotnet
  /BaseFaq.Faq.AI.Generation.Api
  /BaseFaq.Faq.AI.Generation.Business.Generation
  /BaseFaq.Faq.AI.Generation.Business.Worker
  /BaseFaq.Faq.AI.Generation.Test.IntegrationTests
  /BaseFaq.Faq.AI.Matching.Api
  /BaseFaq.Faq.AI.Matching.Business.Matching
  /BaseFaq.Faq.AI.Matching.Business.Worker
  /BaseFaq.Faq.AI.Matching.Test.IntegrationTests
  /BaseFaq.Faq.AI.Common.Providers
  /BaseFaq.Faq.AI.Common.VectorStore
  /BaseFaq.Faq.AI.Common.Contracts
```

Suggested concrete project names:
```text
dotnet/BaseFaq.Faq.AI.Generation.Api/BaseFaq.Faq.AI.Generation.Api.csproj
dotnet/BaseFaq.Faq.AI.Generation.Business.Generation/BaseFaq.Faq.AI.Generation.Business.Generation.csproj
dotnet/BaseFaq.Faq.AI.Generation.Business.Worker/BaseFaq.Faq.AI.Generation.Business.Worker.csproj
dotnet/BaseFaq.Faq.AI.Generation.Test.IntegrationTests/BaseFaq.Faq.AI.Generation.Test.IntegrationTests.csproj

dotnet/BaseFaq.Faq.AI.Matching.Api/BaseFaq.Faq.AI.Matching.Api.csproj
dotnet/BaseFaq.Faq.AI.Matching.Business.Matching/BaseFaq.Faq.AI.Matching.Business.Matching.csproj
dotnet/BaseFaq.Faq.AI.Matching.Business.Worker/BaseFaq.Faq.AI.Matching.Business.Worker.csproj
dotnet/BaseFaq.Faq.AI.Matching.Test.IntegrationTests/BaseFaq.Faq.AI.Matching.Test.IntegrationTests.csproj

dotnet/BaseFaq.Faq.AI.Common.Providers/BaseFaq.Faq.AI.Common.Providers.csproj
dotnet/BaseFaq.Faq.AI.Common.VectorStore/BaseFaq.Faq.AI.Common.VectorStore.csproj
dotnet/BaseFaq.Faq.AI.Common.Contracts/BaseFaq.Faq.AI.Common.Contracts.csproj
```

## Main Risks and Mitigations
| Risk | Impact | Mitigation |
|---|---|---|
| Worker processing without HTTP tenant context | Wrong tenant data access | Include tenant metadata in event and resolve context in worker scope |
| Duplicate event delivery | Duplicate generation/persistence | Idempotency keys + processed-message table + unique constraints |
| LLM quality drift | Low trust in generated FAQs | Prompt versioning + quality checks + human approval gate |
| Provider outages/rate limits | Latency and failures | Retry/backoff, circuit breaker, fallback model strategy |
| Secret leakage | Security incident | Secret manager, strict redaction, no secrets in source config |
| Queue backlog growth | SLA degradation | Queue depth alerts, scaling workers, backpressure controls |

## Final Technical Checklist
- [x] `BaseFaq.Faq.AI` root folder and projects created.
- [x] `Generation` and `Matching` projects follow existing `Api/Business/Test` conventions.
- [ ] Existing API hosts register new AI features without changing current boundaries.
- [ ] AI lifecycle entities and migrations added to FAQ persistence.
- [ ] Async generation event flow implemented end-to-end.
- [ ] Matching endpoint implemented with synchronous response and fallback behavior.
- [ ] Idempotency key support and dedupe table in place.
- [ ] Retry and DLQ policies configured and validated.
- [ ] Distributed tracing across API, broker, worker, DB, provider enabled.
- [ ] Monitoring dashboard and alerts configured.
- [ ] Secret manager integration and key rotation process implemented.
- [ ] Logging redaction rules validated (no key leakage).
- [ ] Prompt governance and quality gate process documented.
- [ ] MVP, hardening, and scale backlog items created and tracked.
