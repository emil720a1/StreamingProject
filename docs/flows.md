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

During login, the client sends credentials, the service finds the user, verifies the password, creates an access token and creates a refresh token.

## Refresh Token Flow

```text
Client
  → AuthController
  → UserService
  → JwtProvider
  → Client
```

The backend validates the refresh token, checks expiration and revocation status, then generates a new access token.

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
