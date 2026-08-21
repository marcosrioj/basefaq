# Seed Tool

## Purpose

`Querify.Tools.Seed` prepares the local or target databases with either essential platform data or realistic sample module data.

It is designed to work across both the tenant database and module databases, which is why it is the standard entrypoint instead of ad hoc SQL or hand-written scripts.

## What it seeds

### Essential data

Essential seed creates or ensures:

- the seed tenant users in `TenantDb`
- the default QnA seed tenant metadata in `TenantDb`
- tenant-user membership
- current module tenant connections for `QnA`, `Direct`, and `Broadcast`

This is the minimum required tenant-side data for tenant configuration and module database routing to work correctly. The seed tenant remains a QnA workspace; Direct and Broadcast route through `TenantConnections`.

### Sample data

Sample seed creates realistic QnA data in the QnA database for the essential seed tenant. Current sample data target: `QnA`.

Direct and Broadcast are included for schema readiness and tenant routing, but the seed tool does not create Direct/Broadcast sample conversations, contacts, threads, or items yet.

It also seeds billing sample scenarios in `TenantDb` covering five tenants with varied states:

| Tenant | Plan | Status |
|---|---|---|
| NorthPeak Analytics | `pro-monthly` | Active and healthy, two paid invoices |
| Pacific Trail Studio | `starter-monthly` | Trialing, no payment yet, trial active |
| MapleForge Media | `pro-monthly` | PastDue, latest payment failed, in grace period |
| Aurora Clinic Systems | `pro-yearly` | Canceled, historical invoices, entitlement inactive |
| BlueHarbor Legal | `business-monthly` | Active, webhook and email outbox troubleshooting demo |

Billing seed also includes five `BillingWebhookInbox` rows, covering completed, failed, and pending cases, and three `EmailOutbox` rows, covering pending, completed, and failed-retryable scenarios.

## Configuration source

The tool reads:

- `ConnectionStrings:TenantDb`
- `ConnectionStrings:QnADb`
- `ConnectionStrings:DirectDb`
- `ConnectionStrings:BroadcastDb`

from `dotnet/Querify.Tools.Seed/appsettings.json`.

## Run the tool

```bash
dotnet run --project dotnet/Querify.Tools.Seed
```

## Menu options

At startup the tool offers these actions:

1. seed realistic sample QnA data
2. seed essential data and ensure QnA/Direct/Broadcast schemas
3. seed Big Data performance rows in QnA
4. clean databases and seed essential plus realistic QnA data
5. clean Seed Big Data rows only
6. clean `TenantDb` only
7. clean `QnADb` only
8. clean `TenantDb`, `QnADb`, `DirectDb`, and `BroadcastDb`
9. clean `DirectDb` only
10. clean `BroadcastDb` only
0. exit

## Recommended choices

### First-time setup

Choose `4` if you want a clean environment with sample content.

### Essential-only setup

Choose `2` if you want all required `TenantDb` seed data and module schemas without the QnA sample content.

### Sample-only setup

Choose `1` for realistic QnA data. It also ensures the essential tenant metadata and Direct/Broadcast routing.

### Resetting local state

Choose `6` when you want to clear only `TenantDb`.

Choose `7` when you want to clear only `QnADb`.

Choose `8` when you want to clear tenant metadata and all current local module databases.

## Safety behavior

- the tool logs the tenant and module connection info it is using
- it applies EF Core migrations before seeding
- essential seed keeps QnA, Direct, and Broadcast tenant routing in sync with appsettings
- it asks for confirmation before appending data into non-empty databases
- seeded module rows must satisfy the same `DbContext/TenantIntegrity` rules as runtime writes
- sample data that creates tenant-owned relationships must use referenced records from the same tenant

## Recommended order of operations

1. start infrastructure
2. on a clean environment, run the seed tool or manually migrate `TenantDbContext`
3. use the migration tool later when you need to apply supported module schema updates across tenant module databases
4. run the APIs and frontend

## Related documents

- [`migration-tool.md`](migration-tool.md)
- [`local-development.md`](local-development.md)
- [`../architecture/querify-tenant-worker.md`](../architecture/querify-tenant-worker.md)
