# BaseFaq

Setup guide for a clean machine.

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

## 2) Apply EF Core migrations (identity DB)
If the tables are not created yet, run:

```bash
dotnet tool install --global dotnet-ef
```

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Identity.Persistence.IdentityDb \
  --startup-project dotnet/BaseFaq.Identity.App
```

The connection string used is in `dotnet/BaseFaq.Identity.App/appsettings.json`.

## 3) Run the Identity API locally

```bash
dotnet run --project dotnet/BaseFaq.Identity.App
```

Endpoints:
- HTTP: `http://localhost:6000`
- HTTPS: `https://localhost:6001`

## 4) (Optional) Run the Identity API in Docker

```bash
docker compose -f docker/docker-compose.yml up -d --build
```

If you do this, ensure the PostgreSQL connection string points to the base services container (not `localhost`).

## Service Ports
- PostgreSQL: `localhost:5432` (databases `bf_identity_db`, `bf_tenant_db`, `bf_faq_db`)
- SMTP4Dev UI: `http://localhost:4590` (SMTP on `1025`)
- RabbitMQ UI: `http://localhost:15672` (AMQP on `5672`, auth disabled)
- Redis: `localhost:6379`

## Stop services

```bash
docker compose -p bf_baseservices -f docker/docker-compose.baseservices.yml down
```

```bash
docker compose -f docker/docker-compose.yml down
```
