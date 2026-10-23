# StreamingProject RTMP load tester

The load tester starts one FFmpeg process per stream key and publishes synthetic
audio/video to the RTMP server. It is intentionally opt-in and is not started by
the default Docker Compose stack.

## Build

From the repository root:

```bash
docker build \
  -f tools/StreamingProject.LoadTests/Dockerfile \
  -t streamingproject-load-tests \
  .
```

## Run one client

Use a fresh, valid stream key:

```bash
docker run --rm \
  --add-host=host.docker.internal:host-gateway \
  streamingproject-load-tests \
  --endpoint rtmp://host.docker.internal:1935/live \
  --stream-keys YOUR_FRESH_STREAM_KEY \
  --duration 10 \
  --clients 1
```

## Run through Docker Compose

The Compose profile uses the service name `backend` for RTMP networking:

```bash
docker compose --profile loadtest run --rm load-tester \
  --endpoint rtmp://backend:1935/live \
  --stream-keys YOUR_FRESH_STREAM_KEY_1,YOUR_FRESH_STREAM_KEY_2 \
  --duration 30 \
  --clients 2
```

Replace the placeholder values with fresh valid stream keys before running the
commands. Each concurrent client requires its own fresh valid stream key. A completed
stream key cannot be reused by the backend.

## Results

The process exits with code `0` only when every client exits successfully. The
output prefixes each FFmpeg line with the client number so concurrent runs can
be distinguished.
