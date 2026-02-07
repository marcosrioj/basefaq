# BaseFaq

Setup guide for a clean machine.

## What’s in this repo
- FAQ Web API
- Tenant Web API
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
  --startup-project dotnet/BaseFaq.Tenant.TenantWeb.App
```

Connection strings live in:
- `dotnet/BaseFaq.Tenant.TenantWeb.App/appsettings.json`

DBs (run per tenant):

The app resolves its database connection from the tenant database, using `Tenant.ConnectionString`. You must run the migrations for every tenant connection string.

The `FaqDb` project below is the migrations assembly used for all tenant FaqWeb app databases; the database name is per-tenant.

Example for one tenant connection of FaqWeb:

```bash
dotnet ef database update \
  --project dotnet/BaseFaq.Faq.FaqWeb.Persistence.FaqDb \
  --startup-project dotnet/BaseFaq.Faq.FaqWeb.App \
  --connection "Host=localhost;Port=5432;Database=bf_faq_db;Username=postgres;Password=Pass123$;"
```

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

Tenant Web API:

```bash
dotnet run --project dotnet/BaseFaq.Tenant.TenantWeb.App
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
  - `basefaq.faq.faqweb.app`
  - `basefaq.tenant.tenantweb.app`
- Wires the service to the `bf-network` network created by the base services.
- Uses the repo root as the build context, so run the command from the repo root.

If you run APIs in Docker, ensure connection strings point to the base services container (use `Host=postgres`, not `localhost`).

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
- FAQ Web API (Docker): `http://localhost:5010`
- Tenant Web API (Docker): `http://localhost:5000`

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
Edit `dotnet/BaseFaq.Faq.FaqWeb.App/appsettings.json` and `dotnet/BaseFaq.Tenant.TenantWeb.App/appsettings.json`:
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
