# Observability

The local observability stack consists of four services:

- **Prometheus** collects metrics from the backend;
- **Grafana Loki** stores application logs;
- **Grafana Alloy** reads Docker logs and forwards them to Loki;
- **Grafana** visualizes metrics and logs.

## Start the stack

Run these commands from the repository root:

```bash
docker compose up -d
docker compose ps
```

The local interfaces are:

| Service | URL |
| --- | --- |
| Prometheus | http://localhost:9090 |
| Grafana | http://localhost:3000 |
| Loki API | http://localhost:3100 |

The default local Grafana credentials are `admin` / `admin`.

## Verify metrics

The backend exposes Prometheus metrics at `/metrics` inside the Docker network:

```bash
docker compose exec backend \
  curl -s http://localhost:8080/metrics | head -30
```

Prometheus scrapes the backend target at `http://backend:8080/metrics`.
Open **Status → Targets** in Prometheus and confirm that the
`streaming-backend` target is `UP`.

Useful PromQL queries:

```promql
up{job="streaming-backend"}
```

```promql
sum by (http_route) (
  rate(http_server_request_duration_seconds_count{job="streaming-backend"}[5m])
)
```

```promql
sum(rate(http_server_request_duration_seconds_sum{job="streaming-backend"}[5m]))
/
sum(rate(http_server_request_duration_seconds_count{job="streaming-backend"}[5m]))
```

The last query shows the average HTTP request duration in seconds.

Business stream metrics include:

```promql
rate(streaming_streams_started_total[5m])
```

```promql
streaming_active_streams
```

```promql
histogram_quantile(
  0.95,
  sum by (le) (rate(streaming_stream_duration_seconds_bucket[5m]))
)
```

## Inspect logs in Grafana

1. Open Grafana at http://localhost:3000.
2. Go to **Connections → Data sources**.
3. Select the Loki data source.
4. Open **Explore**.
5. Select the Docker/Loki data source.
6. Run:

```logql
{job="docker"}
```

To inspect backend logs only:

```logql
{job="docker", service_name="streaming-backend"}
```

To find all records belonging to one request, filter by its correlation ID:

```logql
{job="docker"} |= "CorrelationId" |= "25dd9e7988784b4eba9cfda3ab778371"
```

The `CorrelationId` is returned in the `X-Correlation-ID` response header and
is also included in the structured log scope.

## Log fields

Request completion logs contain fields such as:

- `Method`;
- `Path`;
- `StatusCode`;
- `DurationMs`;
- `CorrelationId`;
- `TraceId` and `RequestId` when available.

Authentication failures log diagnostic context without access tokens,
refresh tokens or passwords. RTMP stream keys are also excluded from logs.

## Troubleshooting

Check the service logs when a component is unavailable:

```bash
docker compose logs backend --tail=100
docker compose logs alloy --tail=100
docker compose logs loki --tail=100
docker compose logs prometheus --tail=100
```

If integration tests use Testcontainers, Docker Desktop must be running:

```bash
docker info
```
