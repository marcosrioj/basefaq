# Migration Tool

## Purpose

`Querify.Tools.Migration` is the tenant-aware migration runner for Querify module databases. It reads tenant metadata first, then applies supported module migrations to the correct tenant databases.

## What it manages

The tool manages supported module databases. Current supported module targets:

- `QnA`
- `Direct`
- `Broadcast`

It uses the tenant database to discover which module database connection strings exist and then applies migrations across those databases. For a tenant whose primary module is QnA, sibling module databases are discovered through current `TenantConnections` for `Direct` and `Broadcast`.

It does not manage the QnA worker Hangfire storage database. Use [`hangfire-qna-db.md`](hangfire-qna-db.md) for `Querify.QnA.Common.Persistence.HangfireQnaDb` commands.

## How it works

1. Load the solution root.
2. Read the tenant database connection string.
3. Choose a module.
4. Choose a command.
5. Either:
   - add a new EF Core migration, or
   - run database update across all tenant databases for the selected module.

## Interactive usage

```bash
dotnet run --project dotnet/Querify.Tools.Migration
```

The tool prompts for:

- module (`QnA`, `Direct`, or `Broadcast`)
- migration command
- migration name when you choose `migrations-add`

## CLI usage

### Apply QnA database updates

```bash
dotnet run --project dotnet/Querify.Tools.Migration -- --module QnA --command database-update
```

### Apply Direct database updates

```bash
dotnet run --project dotnet/Querify.Tools.Migration -- --module Direct --command database-update
```

### Apply Broadcast database updates

```bash
dotnet run --project dotnet/Querify.Tools.Migration -- --module Broadcast --command database-update
```

### Add a new QnA migration

```bash
dotnet run --project dotnet/Querify.Tools.Migration -- --module QnA --command migrations-add --migration-name AddExampleChange
```

Use the same pattern with `--module Direct` or `--module Broadcast` when the schema change belongs to those persistence projects.

## Configuration source

The tool reads the tenant database connection through the repository configuration used by the solution, ultimately relying on the tenant-side configuration rather than hardcoding a second migration-only environment model.

Operationally, that means:

- the tenant database must already be reachable
- tenant records or current `TenantConnections` must contain the relevant module database connection strings
- `dotnet/Querify.Tools.Seed/appsettings.json` carries the local fallback connection strings for QnA, Direct, and Broadcast design-time scaffolding

## Recommended workflow

### On a fresh local environment

1. Start the base services.
2. Run the seed tool first, or manually migrate `TenantDbContext`:

```bash
dotnet ef database update \
  --project dotnet/Querify.Common.EntityFramework.Tenant \
  --startup-project dotnet/Querify.Tenant.BackOffice.Api
```

3. Run the migration tool with `database-update` when tenant metadata already exists.

The seed tool ensures current `TenantConnections` for QnA, Direct, and Broadcast. If you skip the seed tool, create those tenant-side records before using `database-update` for Direct or Broadcast.

### When introducing a schema change

1. make the EF model change in the correct persistence project
2. update the owning module `DbContext/TenantIntegrity` rule when the schema adds or changes a tenant-owned relationship
3. add the migration with `migrations-add`
4. apply the update locally
5. run the relevant integration tests

Tenant-integrity code is part of the model change, not a later command-handler task. The migration should represent the schema; the owning `DbContext` should enforce cross-tenant relationship validity before the changed schema is used.

## Common failure cases

- tenant database is not reachable
- tenant metadata does not have the expected module database connection string
- the solution root cannot be located
- `migrations-add` is used without `--migration-name` in CLI mode

## Related documents

- [`seed-tool.md`](seed-tool.md)
- [`hangfire-qna-db.md`](hangfire-qna-db.md)
- [`local-development.md`](local-development.md)
- [`../architecture/dotnet-backend-overview.md`](../architecture/dotnet-backend-overview.md)
