# Local deployment

The local environment runs behind an Nginx reverse proxy. Nginx is the only
HTTP entry point for the frontend and backend services.

## Start the environment

Run the commands from the repository root:

```bash
docker compose up --build -d
docker compose ps
```

The expected services are:

- `streaming-proxy` — reverse proxy;
- `streaming-frontend` — Angular application;
- `streaming-backend` — ASP.NET Core API;
- `streaming-postgres` — PostgreSQL database.

## Exposed ports

The reverse proxy exposes:

- HTTP: `http://localhost:8080`;
- HTTPS: `https://localhost:8443`;
- RTMP ingest: `localhost:1935`.

The frontend, backend HTTP port, and PostgreSQL port are available only inside
the Docker network. HTTP requests to the backend must go through the proxy.

## HTTPS

Local HTTPS uses a self-signed certificate generated for `localhost`. The
certificate and private key are stored locally under `deploy/nginx/certs/` and
must not be committed to Git.

Because the certificate is not trusted by the operating system, use `-k` with
`curl` during local testing:

```bash
curl -k -i https://localhost:8443/health
```

The expected response is `200 Healthy`.

HTTP requests are redirected to HTTPS:

```bash
curl -i http://localhost:8080
```

For production, replace the local self-signed certificate with a trusted
certificate issued for the deployment domain.

## Reverse proxy routes

Nginx forwards requests to the following internal services:

| Route | Destination | Purpose |
| --- | --- | --- |
| `/` | `frontend:80` | Angular application |
| `/api/` | `backend:8080` | ASP.NET Core API |
| `/health` | `backend:8080` | Backend health check |
| `/chatHub` | `backend:8080` | SignalR WebSocket connection |
| `/hls/` | `backend:8080` | HLS playlists and segments |

The `/chatHub` route enables HTTP/1.1 and forwards WebSocket upgrade headers.

## Authentication and API usage

The frontend uses the same-origin API path:

```text
https://localhost:8443/api
```

Authentication cookies are sent through the HTTPS proxy. Direct browser calls
to the backend container are not required.

## Stop the environment

```bash
docker compose down
```

The PostgreSQL data volume is preserved by default. To remove containers and
the local database volume explicitly, use the appropriate Docker Compose
volume cleanup command only when local data is no longer needed.
