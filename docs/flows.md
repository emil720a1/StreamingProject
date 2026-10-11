# Application Flows

## Authentication Flow

```text
Client
  → AuthController
  → IUserService
  → UserService
  → UserRepository
  → PostgreSQL
```

During login, the client sends credentials, the service finds the user, verifies
the password, and creates access and refresh tokens. The controller writes both
tokens to `HttpOnly` cookies and returns no tokens in the response body. The
browser sends the cookies automatically because Angular API requests use
`withCredentials: true`. The frontend does not store tokens or send a Bearer
header.

In local `Development`, cookies use `SameSite=Lax` without `Secure` so they work
through `http://localhost:4200`. Outside `Development`, cookies use
`SameSite=None; Secure` and therefore require HTTPS.

## Refresh Token Flow

```text
Client
  → AuthController
  → UserService
  → JwtProvider
  → Client
```

The browser sends the refresh-token cookie to `POST /api/Auth/refresh-token`.
The backend validates it, checks expiration and revocation status, generates new
tokens, and rotates both `HttpOnly` cookies. No token is exposed to frontend
JavaScript.

## Logout Flow

```text
Client
  → POST /api/Auth/logout
  → Backend expires access and refresh cookies
  → Protected request returns 401 Unauthorized
```

## Stream Creation Flow

```text
Client
  → StreamsController
  → StreamService
  → StreamRepository
  → StreamingDbContext
  → PostgreSQL
```

The authenticated user sends stream metadata. The application service validates the request, creates a `StreamEntity` and persists it through the repository.

## Joining a Stream

```text
Client
  → StreamsController
  → StreamService
  → StreamRepository
  → UserStream
  → PostgreSQL
```

The backend verifies the user and stream, then creates a `UserStream` record containing `UserId`, `StreamId` and `JoinedAt`.

## Chat Flow

```text
Client
  → SignalR ChatHub
  → ChatService
  → ChatRepository
  → PostgreSQL
```

The client connects to SignalR, joins a stream chat, sends a message, and receives broadcasts from the hub. Messages are persisted as `ChatEntity` records.

## HLS Streaming Flow

```text
Streaming client
  → RTMP endpoint
  → HLS transcoder
  → HLS files
  → Static file middleware
  → Video player
```

The streaming client sends an RTMP stream. The transcoder generates an HLS playlist and media segments. The frontend requests the playlist and plays it in the video player.

## Subscription Flow

```text
Client
  → SubscriptionController
  → SubscriptionService
  → SubscriptionRepository
  → PostgreSQL
```

The backend creates or removes a `SubscriptionEntity` containing the follower ID, followed user ID and subscription creation time.

## Common Backend Request Flow

```text
Controller
  → Application service
  → Repository
  → Entity
  → DbContext
  → PostgreSQL
```

Controllers handle HTTP requests. Application services implement use cases. Repositories handle data access. Entities represent domain data and business rules. The `DbContext` sends database queries to PostgreSQL.
