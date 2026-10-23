# HTTP and SignalR load tester

This tool generates concurrent HTTP requests and SignalR connections against the
streaming backend. It is opt-in and does not start with the default Compose stack.

## Build

```bash
docker build \
  -f tools/StreamingProject.HttpSignalRLoadTests/Dockerfile \
  -t streamingproject-http-signalr-load-tests \
  .
```

## Run HTTP load

```bash
docker run --rm \
  --network container:streaming-backend \
  streamingproject-http-signalr-load-tests \
  --endpoint http://localhost:8080 \
  --mode http \
  --http-path /health \
  --duration 30 \
  --clients 10 \
  --interval-ms 100
```

## Run SignalR load

```bash
docker run --rm \
  --network container:streaming-backend \
  streamingproject-http-signalr-load-tests \
  --endpoint http://localhost:8080 \
  --mode signalr \
  --hub-path /chatHub \
  --stream-id load-test \
  --duration 30 \
  --clients 10
```

## Run through Docker Compose

```bash
docker compose --profile http-signalr-loadtest run --rm http-signalr-load-tester \
  --endpoint http://backend:8080 \
  --mode all \
  --http-path /health \
  --hub-path /chatHub \
  --stream-id load-test \
  --duration 30 \
  --clients 10 \
  --interval-ms 100
```

Use `--mode all` to run both workloads at the same time. The process exits with
code `0` when all clients complete successfully.
