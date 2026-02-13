# BaseFaq

BaseFaq is a multi-tenant FAQ platform with APIs for tenant administration, authenticated portal workflows, and public FAQ access. It uses shared infrastructure components and local Docker services to run the full stack in development.

## What’s in this repo
- FAQ Portal API
- Tenant Back Office API
- Tenant Portal API
- Shared infrastructure libraries (Swagger/OpenAPI, Sentry, MediatR logging, API error handling)
- Base services via Docker (PostgreSQL, RabbitMQ, Redis, SMTP4Dev)

## Prerequisites
- Docker Engine + Docker Compose v2
- .NET SDK `10.0.100` (see `global.json`)
- Optional: `dotnet-ef` tool if you want to apply migrations manually
- Helper scripts default `REDIS_PASSWORD` to `RedisTempPassword`
- If you run Docker Compose manually, export/set `REDIS_PASSWORD` first
  (must match `Redis:Password` in `appsettings.json`)

## Quick start (clean machine)

```bash
dotnet restore BaseFaq.sln
./docker-base.sh
dotnet run --project dotnet/BaseFaq.Faq.Common.Persistence.Seed
dotnet run --project dotnet/BaseFaq.Faq.Portal.Api
```

For Windows PowerShell, use `.\docker-base.ps1`.

## 0) Restore and build

```bash
dotnet restore BaseFaq.sln
dotnet build BaseFaq.sln
```

## 1) Start base services (PostgreSQL, RabbitMQ, Redis, SMTP)
From the repo root:

macOS / Linux:

```bash
./docker-base.sh
```

Windows (PowerShell):

```powershell
.\docker-base.ps1
```

Notes:
- The script only stops/removes containers in the `bf_baseservices` compose project.
- It starts the base services using `docker/docker-compose.baseservices.yml` and creates the `bf_tenant_db` and `bf_faq_db` databases.
- PostgreSQL password is `Pass123$` (the compose file uses `$$` to escape `$`).
- If PowerShell blocks script execution, run:
  `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

If you prefer to run Docker Compose manually:

```bash
export REDIS_PASSWORD=RedisTempPassword
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
  --startup-project dotnet/BaseFaq.Tenant.BackOffice.Api
```

Connection strings live in:
- `dotnet/BaseFaq.Tenant.BackOffice.Api/appsettings.json`

App DBs (run per tenant):

Use the migrations console app and follow the prompts:

```bash
dotnet run --project dotnet/BaseFaq.Migration
```

Notes:
- The console app asks for the target `AppEnum` and whether to run `Migrations add` or `Database update`.
- `Database update` applies migrations for **all** tenant connection strings in `Tenant.ConnectionString` filtered by the chosen app.
- It reads the tenant DB connection string from `dotnet/BaseFaq.Tenant.BackOffice.Api/appsettings.json`
  (`ConnectionStrings:TenantDb`).
- When creating a new migration, make sure the current tenant connection is properly added.
- Migrations run against all existing tenants for the selected app.

## Seed data
The seed app inserts realistic data into both the tenant and FAQ databases.

It reads connection strings from:
- `dotnet/BaseFaq.Faq.Common.Persistence.Seed/appsettings.json`

Run the seed:

```bash
dotnet run --project dotnet/BaseFaq.Faq.Common.Persistence.Seed
```

Notes:
- The seed logs which `TenantDb` and `FaqDb` connections it uses from `appsettings.json`.
- On startup it offers actions: seed, clean+seed, or clean-only.
- If the target database already has data, it will ask whether to append.
- It creates dozens of records per entity, including child entities (items, tags, votes, etc.).

## Hostname that works on host + Docker
If you want a single hostname that works both in Rider (host machine) and inside Docker,
use `host.docker.internal`.

Linux needs two small steps (Windows/macOS already provide this name):

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
- HTTPS: `https://localhost:5011`

Swagger / OpenAPI (FAQ app, Development only):
- Swagger UI: `/swagger`
- Swagger JSON: `/swagger/v1/swagger.json`
- OpenAPI JSON (minimal API): `/openapi/v1.json`

Tenant Back Office API:

```bash
dotnet run --project dotnet/BaseFaq.Tenant.BackOffice.Api
```

Endpoints:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Tenant Portal API:

```bash
dotnet run --project dotnet/BaseFaq.Tenant.Portal.Api
```

Endpoints:
- HTTP: `http://localhost:5002`
- HTTPS: `https://localhost:5003`

FAQ Public API:

```bash
dotnet run --project dotnet/BaseFaq.Faq.Public.Api
```

Endpoints:
- HTTP: `http://localhost:5020`
- HTTPS: `https://localhost:5021`

## 4) (Optional) Run API in Docker
APIs (Docker):

```bash
docker compose -p bf_services -f docker/docker-compose.yml up -d --build
```

This compose file:
- Runs these services:
  - `basefaq.faq.portal.api`
  - `basefaq.tenant.backoffice.api`
  - `basefaq.tenant.portal.api`
  - `basefaq.faq.public.api`
- Wires the service to the `bf-network` network created by the base services.
- Uses the repo root as the build context, so run the command from the repo root.

If you run APIs in Docker, this repo defaults to `host.docker.internal` in `appsettings.json` so the same values work for host + Docker.

You can also use the helper script:

macOS / Linux:

```bash
./docker.sh
```

Windows (PowerShell):

```powershell
.\docker.ps1
```

Note: the script removes the BaseFaq Docker images and prunes dangling Docker images after it brings the stack up.

## Service Ports
- PostgreSQL: `localhost:5432` (databases `bf_tenant_db`, `bf_faq_db`)
- SMTP4Dev UI: `http://localhost:4590` (SMTP on `1025`)
- RabbitMQ UI: `http://localhost:15672` (AMQP on `5672`, auth disabled)
- Redis: `localhost:6379`
- FAQ Portal API (Docker): `http://localhost:5010`
- Tenant Back Office API (Docker): `http://localhost:5000`
- Tenant Portal API (Docker): `http://localhost:5002`
- FAQ Public API (Docker): `http://localhost:5020`

## Redis cache
Clear all Redis databases:

```bash
redis-cli FLUSHALL
```

Clear only the current database:

```bash
redis-cli FLUSHDB
```

If you need host/port/auth:

```bash
redis-cli -h <host> -p <port> -a <password> FLUSHALL
```

## Tests
Integration tests:

```bash
dotnet test dotnet/BaseFaq.Faq.Portal.Test.IntegrationTests/BaseFaq.Faq.Portal.Test.IntegrationTests.csproj
dotnet test dotnet/BaseFaq.Faq.Public.Test.IntegrationTests/BaseFaq.Faq.Public.Test.IntegrationTests.csproj
dotnet test dotnet/BaseFaq.Tenant.BackOffice.Test.IntegrationTests/BaseFaq.Tenant.BackOffice.Test.IntegrationTests.csproj
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
- Allowed Callback URLs: `http://localhost:5010/swagger/oauth2-redirect.html`, `http://localhost:5000/swagger/oauth2-redirect.html`, `http://localhost:5002/swagger/oauth2-redirect.html`
- Allowed Web Origins: `http://localhost:5010`, `http://localhost:5000`, `http://localhost:5002`
- Ensure the app is public (no client secret required)
- In the app's **APIs** tab, authorize access to your API identifier (Audience)

### 3) Configure BaseFaq apps
Edit `dotnet/BaseFaq.Faq.Portal.Api/appsettings.json`, `dotnet/BaseFaq.Tenant.BackOffice.Api/appsettings.json`, and `dotnet/BaseFaq.Tenant.Portal.Api/appsettings.json`:
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
- Protected APIs require:
  - `Authorization: Bearer <access_token>`
  - APIs: FAQ Portal, Tenant Back Office, Tenant Portal
- FAQ Portal requires tenant context header:
  - `X-Tenant-Id: <tenant-guid>`
- FAQ Public requires client context header:
  - `X-Client-Key: <client-key>`

## Troubleshooting
- `network bf-network declared as external, but could not be found`:
  run base services first (`./docker-base.sh` or `.\docker-base.ps1`) before `docker/docker-compose.yml`.
- `set REDIS_PASSWORD` error during base services startup:
  use the helper script, or set `REDIS_PASSWORD` manually before `docker compose`.
- HTTPS local cert warning/failure:
  run `dotnet dev-certs https --trust` once on your machine.

## Stop services

```bash
docker compose -p bf_baseservices -f docker/docker-compose.baseservices.yml down
```

```bash
docker compose -f docker/docker-compose.yml down
```
