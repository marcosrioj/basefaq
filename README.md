# BaseFaq

Setup guide for a clean machine.

## What’s in this repo
- FAQ Portal API
- Tenant Portal API
- Shared infrastructure libraries (Swagger/OpenAPI, Sentry, MediatR logging, API error handling)
- Base services via Docker (PostgreSQL, RabbitMQ, Redis, SMTP4Dev)

## Prerequisites
- Docker Engine + Docker Compose v2
- .NET SDK `10.0.100` (see `global.json`)
- Optional: `dotnet-ef` tool if you want to apply migrations manually
- Set `REDIS_PASSWORD` in docker-base.sh (must match `Redis:Password` in `appsettings.json`)

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

Tenant DB (stores tenant records and each tenant's connection string):

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Common.EntityFramework.Tenant \
  --startup-project dotnet/BaseFaq.Tenant.Portal.Api
```

Connection strings live in:
- `dotnet/BaseFaq.Tenant.Portal.Api/appsettings.json`

App DBs (run per tenant):

Use the migrations console app and follow the prompts:

```bash
dotnet run --project dotnet/BaseFaq.Migration
```

Notes:
- The console app asks for the target `AppEnum` and whether to run `Migrations add` or `Database update`.
- `Database update` applies migrations for **all** tenant connection strings in `Tenant.ConnectionString` filtered by the chosen app.
- It reads the tenant DB connection string from `dotnet/BaseFaq.Tenant.Portal.Api/appsettings.json`
  (`ConnectionStrings:TenantDb`).

## Hostname that works on host + Docker
If you want a single hostname that works both in Rider (host machine) and inside Docker,
use `host.docker.internal`.

Linux needs two small steps:

1) Add the host alias for Docker containers (already included in `docker/docker-compose.yml`):

```yaml
extra_hosts:
  - "host.docker.internal:host-gateway"
```

2) Map the name on your host machine (Linux only):

```bash
echo "127.0.0.1 host.docker.internal" | sudo tee -a /etc/hosts
```

Then you can use this in tenant connection strings:

```
Host=host.docker.internal;Port=5432;Database=bf_faq_db;Username=postgres;Password=Pass123$;
```

## 3) Run the API locally
FAQ Portal API:

```bash
dotnet run --project dotnet/BaseFaq.Faq.Portal.Api
```

Endpoints:
- HTTP: `http://localhost:5010`

Swagger / OpenAPI (FAQ app, Development only):
- Swagger UI: `/swagger`
- Swagger JSON: `/swagger/v1/swagger.json`
- OpenAPI JSON (minimal API): `/openapi/v1.json`

Tenant Portal API:

```bash
dotnet run --project dotnet/BaseFaq.Tenant.Portal.Api
```

Endpoints:
- HTTP: `http://localhost:5000`

Note: both apps default to port 5000 in Development, so change one if you run both locally.

## 4) (Optional) Run API in Docker
APIs (Docker):

```bash
docker compose -p bf_services -f docker/docker-compose.yml up -d --build
```

This compose file:
- Runs these services:
  - `basefaq.faq.portal.app`
  - `basefaq.tenant.portal.app`
- Wires the service to the `bf-network` network created by the base services.
- Uses the repo root as the build context, so run the command from the repo root.

If you run APIs in Docker, this repo defaults to `host.docker.internal` in `appsettings.json` so the same values work for host + Docker.

You can also use the helper script:

```bash
./docker.sh
```

Note: `./docker.sh` removes the BaseFaq Docker images and prunes Docker images after it brings the stack up.

## Service Ports
- PostgreSQL: `localhost:5432` (databases `bf_tenant_db`, `bf_faq_db`)
- SMTP4Dev UI: `http://localhost:4590` (SMTP on `1025`)
- RabbitMQ UI: `http://localhost:15672` (AMQP on `5672`, auth disabled)
- Redis: `localhost:6379`
- FAQ Portal API (Docker): `http://localhost:5010`
- Tenant Portal API (Docker): `http://localhost:5000`

## Tests
Integration tests:

```bash
dotnet test dotnet/BaseFaq.Faq.Portal.Test.IntegrationTests/BaseFaq.Faq.Portal.Test.IntegrationTests.csproj
dotnet test dotnet/BaseFaq.Tenant.Portal.Test.IntegrationTests/BaseFaq.Tenant.Portal.Test.IntegrationTests.csproj
```

## Auth0 setup (step-by-step)
You must use an external identity provider. This project expects Auth0 to issue JWTs.

### 1) Create an Auth0 API
Create a new API:
- Name: `BaseFaq API`
- Identifier (Audience): `https://<API_IDENTIFIER>`

### 2) Create an Auth0 application (SPA for Swagger UI)
Create a Single Page Application:
- Allowed Callback URLs: `http://localhost:5010/swagger/oauth2-redirect.html`, `http://localhost:5000/swagger/oauth2-redirect.html`
- Allowed Web Origins: `http://localhost:5010`, `http://localhost:5000`
- Ensure the app is public (no client secret required)
- In the app's **APIs** tab, authorize access to your API identifier (Audience)

### 3) Configure BaseFaq apps
Edit `dotnet/BaseFaq.Faq.Portal.Api/appsettings.json` and `dotnet/BaseFaq.Tenant.Portal.Api/appsettings.json`:
- `JwtAuthentication:Authority` = `https://<AUTH0_DOMAIN>/`
- `JwtAuthentication:Audience` = `https://<API_IDENTIFIER>`
- `Session:UserIdClaimType` = `sub`
- `SwaggerOptions:swaggerAuth:AuthorizeEndpoint` = `https://<AUTH0_DOMAIN>/authorize`
- `SwaggerOptions:swaggerAuth:TokenEndpoint` = `https://<AUTH0_DOMAIN>/oauth/token`
- `SwaggerOptions:swaggerAuth:Audience` = `https://<API_IDENTIFIER>`
- `SwaggerOptions:swaggerAuth:ClientId` = `<AUTH0_CLIENT_ID>`

Use `<AUTH0_DOMAIN>` from your Auth0 tenant (for example, `your-tenant.us.auth0.com`).

### 4) Include name/email in access tokens (optional)
Auth0 does not add `name`/`email` to access tokens by default. If you need them in API calls,
add an Action that injects namespaced claims:

```js
// Auth0 Action (Post Login)
exports.onExecutePostLogin = async (event, api) => {
  const ns = 'https://basefaq.com/';
  if (event.user.name) {
    api.accessToken.setCustomClaim(`${ns}name`, event.user.name);
  }
  if (event.user.email) {
    api.accessToken.setCustomClaim(`${ns}email`, event.user.email);
  }
};
```

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
