# StreamingProject

StreamingProject is a streaming platform built with ASP.NET Core, Entity Framework Core and PostgreSQL.

## Main Features

- user registration and authentication;
- JWT access tokens and refresh tokens;
- live streams;
- stream participants;
- SignalR chat;
- recorded videos and HLS playback;
- user subscriptions;
- roles and permissions.

## Solution Structure

| Project | Responsibility |
|---|---|
| `StreamingProject.Domain` | Domain entities, enums and business rules |
| `StreamingProject.Contracts` | API request and response DTOs |
| `StreamingProject.Application` | Application services and use cases |
| `StreamingProject.Infrastructure.Postgres` | EF Core, PostgreSQL, repositories and infrastructure services |
| `StreamingProject.Presenters` | API controllers and SignalR hubs |
| `StreamingProject.Web` | Application startup and HTTP pipeline |
| `Shared` | Shared failures, exceptions, mappers and utilities |

## Prerequisites

Install the following tools before starting the project:

- .NET SDK required by `global.json`;
- PostgreSQL;
- Rider or another .NET-compatible IDE;
- Git.

## Local Setup

Clone the repository and move into its directory:

```bash
git clone https://github.com/emil720a1/StreamingProject.git
cd StreamingProject
```

Restore dependencies:

```bash
dotnet restore
```

Configure the local database connection and authentication settings using environment variables or a local, untracked `StreamingProject.Web/appsettings.Local.json` file. Safe templates are available in:

- `StreamingProject.Web/appsettings.example.json`;
- `.env.example`.

Never commit passwords, JWT signing keys, or other environment-specific secrets.

Build the solution:

```bash
dotnet build
```

Run the web application:

```bash
dotnet run --project StreamingProject.Web
```

The exact HTTP and HTTPS ports are defined by the web project's launch settings and environment configuration.

## Docker Compose

Docker Compose provides a reproducible local environment with the Angular frontend,
ASP.NET Core backend and PostgreSQL database.

Create the local environment file from the safe template:

```bash
cp .env.example .env
```

Start all services:

```bash
docker compose up --build
```

The services are available at:

- frontend: `http://localhost:4200`;
- backend and Swagger: `http://localhost:5228/swagger/index.html`;
- PostgreSQL: `localhost:5432`;
- RTMP server: `localhost:1935`.

The PostgreSQL data is stored in the `postgres-data` Docker volume and survives
container restarts. Stop the services with:

```bash
docker compose down
```

To remove the database volume as well, use the following only when local data can
be discarded:

```bash
docker compose down -v
```

View service logs with:

```bash
docker compose logs -f backend
```

## Database

The backend uses Entity Framework Core with PostgreSQL.

The database contains data for:

- users;
- streams;
- chat messages;
- videos;
- subscriptions;
- participants;
- roles and permissions;
- refresh tokens.

Apply migrations according to the configured database connection:

```bash
dotnet ef database update --project StreamingProject.Infrastructure.Postgres --startup-project StreamingProject.Web
```

## Authentication

The browser client uses cookie-based authentication. The backend still issues
JWT access tokens and refresh tokens internally, but both tokens are transported
only in `HttpOnly` cookies. Login and token-refresh responses do not return the
tokens in JSON. Angular neither reads nor stores them in `localStorage`, and it
does not add an `Authorization: Bearer` header. The API intentionally reads the
access token only from its cookie; Bearer headers are not a fallback
authentication mechanism.

### Authentication cookies in local Docker

The Docker Compose environment serves the frontend at `http://localhost:4200` and
the backend at `http://localhost:5228`. Because this local environment uses HTTP,
authentication cookies are configured differently depending on the application
environment:

- in `Development`, both authentication cookies are `HttpOnly`, use
  `SameSite=Lax` and do not use the `Secure` flag, so the browser can send them
  over local HTTP;
- outside `Development`, both cookies use `SameSite=None` and always have the
  `Secure` flag, so they can only be sent over HTTPS.

The Angular client sends API requests with credentials, and the backend CORS
policy explicitly allows credentials from `http://localhost:4200`. The access
cookie authenticates protected requests. The refresh cookie is sent only to
`POST /api/Auth/refresh-token`, which rotates both cookies. Logging out calls
`POST /api/Auth/logout`, which expires both cookies. `GET /api/Auth/session`
allows Angular to check the server-side session without reading either token.

To verify the local authentication flow, start Docker Compose, log in through
`http://localhost:4200`, confirm that authenticated API requests succeed, and
then log out. After logout, the same protected request must return `401
Unauthorized`.

The authentication flow is documented in [docs/flows.md](docs/flows.md).

## Streaming and Chat

Live streams are received through the RTMP pipeline and converted to HLS for playback. Chat communication uses SignalR, while chat messages are persisted in PostgreSQL.

See [docs/flows.md](docs/flows.md) for the detailed streaming and chat flows.

## Architecture Documentation

- [Architecture map](docs/architecture.md)
- [Application flows](docs/flows.md)

The architecture document describes backend projects, entities, relationships, foreign keys and known technical issues.

## Testing

Run all tests with:

```bash
dotnet test
```

## Development Guidelines

- keep domain rules inside the domain or application layer;
- keep database access inside repositories and infrastructure;
- use contracts instead of exposing domain entities through the API;
- add tests for new application behavior;
- update the documentation when architecture or startup instructions change;
- do not commit secrets, local configuration files or generated build artifacts.

## Known Issues and Follow-up Work

- verify the complete local startup flow;
- document all environment variables and ports;
- verify refresh-token persistence configuration;
- add explicit EF Core configuration for stream likes;
- review cascade delete behavior before production deployment;
- document and validate the frontend application separately.
