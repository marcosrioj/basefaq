# BaseFaq

Setup guide for a clean machine.

## What’s in this repo
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
- It starts the base services using `docker/docker-compose.baseservices.yml` and creates the `bf_tenant_db` and `bf_faq_db` databases.
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

FAQ DB:

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Faq.FaqWeb.Persistence.FaqDb \
  --startup-project dotnet/BaseFaq.Faq.FaqWeb.App
```

Connection strings live in:
- `dotnet/BaseFaq.Faq.FaqWeb.App/appsettings.json`

Note: the FAQ app defaults to `bf_faq_db` in `appsettings.json`. Update it or override with `ConnectionStrings__FaqDb` to match the created database.

## 3) Run the API locally
FAQ Web API:

```bash
dotnet run --project dotnet/BaseFaq.Faq.FaqWeb.App
```

Endpoints:
- HTTP: `http://localhost:5010`

Swagger / OpenAPI (FAQ app, Development only):
- Swagger UI: `/swagger`
- Swagger JSON: `/swagger/v1/swagger.json`
- OpenAPI JSON (minimal API): `/openapi/v1.json`

## 4) (Optional) Run API in Docker
FAQ API (Docker):

```bash
docker compose -p bf_services -f docker/docker-compose.yml up -d --build
```

This compose file:
- Runs these services:
  - `basefaq.faq.faqweb.app`
- Wires the service to the `bf-network` network created by the base services.
- Uses the repo root as the build context, so run the command from the repo root.

If you run APIs in Docker, ensure connection strings point to the base services container (use `Host=postgres`, not `localhost`).

You can also use the helper script:

```bash
./docker.sh
```

## Service Ports
- PostgreSQL: `localhost:5432` (databases `bf_tenant_db`, `bf_faq_db`)
- SMTP4Dev UI: `http://localhost:4590` (SMTP on `1025`)
- RabbitMQ UI: `http://localhost:15672` (AMQP on `5672`, auth disabled)
- Redis: `localhost:6379`
- FAQ Web API (Docker): `http://localhost:5010`

## Entra (Azure AD) setup (step-by-step)
You must use an external identity provider. This project expects Microsoft Entra to issue JWTs.

### 1) Create Entra app registrations
Create two app registrations:
- API app (represents `BaseFaq.Faq.FaqWeb.App`)
- Client app (Swagger UI or your frontend)

### 2) API app configuration
In **Expose an API**:
- Set **Application ID URI** to `api://<API_APP_CLIENT_ID>`
- Add scope: `BaseFaq.All`

### 3) Client app configuration
In **Authentication**:
- Add **Web** platform
- Redirect URIs:
  - `https://localhost:5011/swagger/oauth2-redirect.html`
  - `http://localhost:5010/swagger/oauth2-redirect.html`

In **API permissions**:
- Add `api://<API_APP_CLIENT_ID>/BaseFaq.All`
- Add `openid`, `profile`

### 4) Configure BaseFaq.Faq.FaqWeb.App
Edit `dotnet/BaseFaq.Faq.FaqWeb.App/appsettings.json`:
- `JwtAuthentication:Authority` = `https://login.microsoftonline.com/<TENANT_ID_OR_COMMON>/v2.0`
- `JwtAuthentication:Audience` = `api://<API_APP_CLIENT_ID>`
- `SwaggerOptions:swaggerAuth:AuthorizeEndpoint` = `https://login.microsoftonline.com/<TENANT_ID_OR_COMMON>/oauth2/v2.0/authorize`
- `SwaggerOptions:swaggerAuth:TokenEndpoint` = `https://login.microsoftonline.com/<TENANT_ID_OR_COMMON>/oauth2/v2.0/token`
- `SwaggerOptions:swaggerAuth:Scopes` = `api://<API_APP_CLIENT_ID>/BaseFaq.All`

Use `<TENANT_ID_OR_COMMON>`:
- Single-tenant: your tenant ID
- Multi-tenant: `common`

### 5) Call the API
All requests must include:
- `Authorization: Bearer <access_token>`
- `X-Tenant-Id: <tenant-guid>`

## Stop services

```bash
docker compose -p bf_baseservices -f docker/docker-compose.baseservices.yml down
```

```bash
docker compose -f docker/docker-compose.yml down
```
