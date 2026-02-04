# BaseFaq

Setup guide for a clean machine.

## What’s in this repo
- Identity API (auth/issuer)
- FAQ Web API
- Shared infrastructure libraries (Swagger/OpenAPI, Sentry, MediatR logging, API error handling)
- Base services via Docker (PostgreSQL, RabbitMQ, Redis, SMTP4Dev)

## Prerequisites
- Docker Engine + Docker Compose v2
- .NET SDK `10.0.100` (see `global.json`)
- Optional: `dotnet-ef` tool if you want to apply migrations manually

## 1) Start base services (PostgreSQL, RabbitMQ, Redis, SMTP)
From the repo root:

```bash
./docker-base.sh
```

Notes:
- The script stops and removes **all** running Docker containers, then prunes Docker state. Use with care.
- It starts the base services using `docker/docker-compose.baseservices.yml` and creates the `bf_identity_db`, `bf_tenant_db`, and `bf_faq_db` databases.
- PostgreSQL password is `Pass123$` (the compose file uses `$$` to escape `$`).

If you prefer to run Docker Compose manually:

```bash
docker compose -p bf_baseservices -f docker/docker-compose.baseservices.yml up -d --wait
```

## 2) Apply EF Core migrations
If the tables are not created yet, install EF tooling:

```bash
dotnet tool install --global dotnet-ef
```

Identity DB:

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Identity.Persistence.IdentityDb \
  --startup-project dotnet/BaseFaq.Identity.App
```

FAQ DB:

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Faq.FaqWeb.Persistence.FaqDb \
  --startup-project dotnet/BaseFaq.Faq.FaqWeb.App
```

Connection strings live in:
- `dotnet/BaseFaq.Identity.App/appsettings.json`
- `dotnet/BaseFaq.Faq.FaqWeb.App/appsettings.json`

Note: the FAQ app defaults to `bf_fad_db` in `appsettings.json`. Update it to `bf_faq_db` or override with `ConnectionStrings__DefaultConnection` to match the created database.

## 3) Run the APIs locally
Identity API:

```bash
dotnet run --project dotnet/BaseFaq.Identity.App
```

Endpoints:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

FAQ Web API (requires Identity API for auth by default):

```bash
dotnet run --project dotnet/BaseFaq.Faq.FaqWeb.App
```

Endpoints:
- HTTP: `http://localhost:5010`
- HTTPS: `https://localhost:5011`

Swagger / OpenAPI (FAQ app, Development only):
- Swagger UI: `/swagger`
- Swagger JSON: `/swagger/v1/swagger.json`
- OpenAPI JSON (minimal API): `/openapi/v1.json`

## 4) (Optional) Run APIs in Docker
Identity + FAQ APIs (Docker):

```bash
docker compose -f docker/docker-compose.yml up -d --build
```

This compose file:
- Runs the Identity API and FAQ Web API.
- Wires both to the `bf-network` network created by the base services.
- Injects connection strings and Identity endpoints for the FAQ app.

If you run APIs in Docker, ensure connection strings point to the base services container (use `Host=postgres`, not `localhost`).

## Service Ports
- PostgreSQL: `localhost:5432` (databases `bf_identity_db`, `bf_tenant_db`, `bf_faq_db`)
- SMTP4Dev UI: `http://localhost:4590` (SMTP on `1025`)
- RabbitMQ UI: `http://localhost:15672` (AMQP on `5672`, auth disabled)
- Redis: `localhost:6379`
- Identity API (Docker): `https://localhost:6000` (mapped from container `5001`)
- FAQ Web API (Docker): `http://localhost:5010`

## Stop services

```bash
docker compose -p bf_baseservices -f docker/docker-compose.baseservices.yml down
```

```bash
docker compose -f docker/docker-compose.yml down
```
