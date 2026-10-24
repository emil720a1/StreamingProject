# Current Monolith Architecture

> Related issue: [#79](https://github.com/emil720a1/StreamingProject/issues/79)

## Purpose

This document describes the current architecture of StreamingProject before the migration to microservices.

The current system is a **modular monolith**: the application is deployed as one ASP.NET Core process, but the code is already divided into separate architectural layers and business modules.

The purpose of this document is to make the current dependencies visible before defining service boundaries.

## High-Level Architecture

```mermaid
flowchart TB
    Frontend[Angular Frontend]
    Publisher[OBS / FFmpeg Publisher]

    Web["StreamingProject.Web<br/>ASP.NET Core process"]

    Presenters["Presenters<br/>REST Controllers<br/>SignalR Hub<br/>RTMP Event Handler"]
    Application["Application<br/>Use Cases and Services"]
    Domain["Domain<br/>Entities and Business Rules"]
    Infrastructure["Infrastructure.Postgres<br/>EF Core, Repositories, JWT, FFmpeg, Storage"]
    DB[(PostgreSQL)]
    HLS[("wwwroot/hls<br/>HLS playlist and segments")]
    Observability["OpenTelemetry<br/>Prometheus / Tempo / Logs"]

    Frontend -->|HTTP| Web
    Frontend -->|SignalR /chatHub| Web
    Publisher -->|RTMP :1935| Web

    Web --> Presenters
    Presenters --> Application
    Application --> Domain
    Application --> Infrastructure
    Infrastructure --> DB
    Infrastructure --> HLS
    Web --> Observability
```

## Runtime Entry Point

The application starts in `StreamingProject.Web/Program.cs`.

The Web project currently configures and hosts all major capabilities:

- ASP.NET Core HTTP pipeline;
- authentication and authorization;
- REST controllers;
- SignalR at `/chatHub`;
- RTMP server on port `1935`;
- HLS static files;
- PostgreSQL and Entity Framework Core;
- health checks at `/health`;
- Prometheus metrics;
- OpenTelemetry traces;
- development and production logging formats.

This means that one deployed process currently owns HTTP, realtime communication, RTMP lifecycle handling and HLS coordination.

## Project Layers

| Project | Responsibility | Examples |
|---|---|---|
| `StreamingProject.Domain` | Domain entities and rules | `StreamEntity`, `UserEntity`, `ChatEntity`, `VideoEntity` |
| `StreamingProject.Contracts` | API request and response DTOs | stream, chat, user and subscription contracts |
| `StreamingProject.Application` | Use cases and business orchestration | `StreamService`, `ChatService`, `UserService` |
| `StreamingProject.Infrastructure.Postgres` | Persistence and external infrastructure | `StreamingDbContext`, repositories, JWT, FFmpeg, storage |
| `StreamingProject.Presenters` | Delivery mechanisms | controllers, SignalR hub, RTMP event handler |
| `StreamingProject.Web` | Composition root and runtime host | `Program.cs`, middleware and dependency injection |
| `Shared` | Cross-cutting primitives | failures, errors, mappings and utilities |

## Main Business Modules

### Identity and Access

Responsible for users, authentication, refresh tokens, roles and permissions.

Main components:

- `AuthController`;
- `UsersController`;
- `RoleController`;
- `PermissionController`;
- JWT and cookie authentication;
- authorization policies such as `Permission.Read` and `Permission.Create`.

Potential future service: **Identity Service**.

### Stream Management

Responsible for stream metadata and lifecycle state.

Main components:

- `StreamsController`;
- `IStreamService` and `StreamService`;
- `IStreamRepository`;
- `StreamEntity`;
- stream participants and stream likes;
- stream status and stream key validation.

Potential future service: **Stream Management Service**.

### Media Ingest and Processing

Responsible for receiving live media and producing HLS output.

Main components:

- LiveStreamingServerNet RTMP server;
- `RtmpServerEventHandler`;
- `IHlsTranscoderService`;
- `LocalHlsTranscoderService`;
- FFmpeg processes;
- `wwwroot/hls` playlist and segment files.

Potential future service: **Media Service**, which is the strongest candidate for the Go service because it is network- and concurrency-oriented.

### Chat

Responsible for chat history, message commands and realtime notifications.

Main components:

- `ChatController`;
- `ChatHub`;
- `ChatService`;
- `ChatRepository`;
- `ChatNotificationService`;
- `ChatEntity`.

Potential future service: **Chat Service**.

### Video Catalog

Responsible for recorded video metadata and video access.

Main components:

- `VideoController`;
- `VideoService`;
- `VideoEntity`;
- video repository.

Potential future service: **Video Catalog Service**.

### Subscriptions

Responsible for following users or streamers.

Main components:

- `SubscriptionController`;
- `SubscriptionService`;
- `SubscriptionEntity`;
- subscription repository.

Potential future service: **Subscription Service**.

## REST Request Flow

Example: creating a stream.

```mermaid
sequenceDiagram
    participant Client as Frontend
    participant Controller as StreamsController
    participant Service as StreamService
    participant Repository as StreamRepository
    participant DbContext as StreamingDbContext
    participant Database as PostgreSQL

    Client->>Controller: POST /api/streams
    Controller->>Controller: Authenticate and authorize
    Controller->>Service: CreateStreamAsync(request)
    Service->>Service: Validate request
    Service->>Service: Create StreamEntity
    Service->>Repository: AddStreamAsync(entity)
    Repository->>DbContext: Add and SaveChanges
    DbContext->>Database: INSERT Streams
    Database-->>DbContext: Saved record
    DbContext-->>Repository: Entity
    Repository-->>Service: StreamEntity
    Service-->>Controller: CreateStreamResponseDto
    Controller-->>Client: HTTP response
```

## RTMP and HLS Flow

```mermaid
sequenceDiagram
    participant Publisher as OBS / FFmpeg
    participant RtmpServer as RTMP Server
    participant Handler as RtmpServerEventHandler
    participant StreamService as StreamService
    participant Database as PostgreSQL
    participant Hls as LocalHlsTranscoderService
    participant Files as wwwroot/hls
    participant Web as StreamingProject.Web
    participant Player as Video Player

    Publisher->>RtmpServer: Publish /live/{streamKey}
    RtmpServer->>Handler: OnRtmpStreamPublishedAsync
    Handler->>StreamService: ValidateStreamKeyAsync
    StreamService->>Database: Check stream key
    Database-->>StreamService: Valid key
    Handler->>StreamService: StartStreamByKeyAsync
    StreamService->>Hls: StartTranscodingAsync
    Hls->>RtmpServer: Read local RTMP stream
    Hls->>Files: Write index.m3u8 and .ts segments
    StreamService->>Database: Mark stream as started
    Player->>Web: GET /hls/{streamId}/index.m3u8
    Web-->>Player: HLS playlist and segments

    Publisher->>RtmpServer: Unpublish
    RtmpServer->>Handler: OnRtmpStreamUnpublishedAsync
    Handler->>StreamService: EndStreamByKeyAsync
    StreamService->>Hls: StopTranscodingAsync
    StreamService->>Database: Mark stream as ended
```

## Chat Flow

The chat currently uses REST for commands and SignalR for realtime delivery.

```mermaid
sequenceDiagram
    participant Client as Chat Client
    participant Controller as ChatController
    participant Service as ChatService
    participant Repository as ChatRepository
    participant Database as PostgreSQL
    participant Notify as ChatNotificationService
    participant Hub as SignalR ChatHub

    Client->>Controller: POST /api/chat/send
    Controller->>Service: SendMessageAsync
    Service->>Repository: Save ChatEntity
    Repository->>Database: INSERT ChatMessages
    Database-->>Repository: Saved message
    Repository-->>Service: ChatEntity
    Service->>Notify: BroadcastMessageAsync
    Notify->>Hub: Send to stream group
    Hub-->>Client: Realtime message
```

`ChatHub` itself manages SignalR connections and stream groups. Message persistence and message business logic are handled by `ChatService`.

## Shared Database Model

All current modules use the same `StreamingDbContext` and PostgreSQL database.

The context currently contains:

- users and Identity tables;
- roles and permissions;
- streams;
- stream participants;
- stream likes;
- chat messages;
- videos;
- subscriptions;
- refresh tokens.

```mermaid
erDiagram
    USER ||--o{ STREAM : creates
    USER ||--o{ CHAT_MESSAGE : sends
    STREAM ||--o{ CHAT_MESSAGE : contains
    STREAM ||--o{ USER_STREAM : has
    USER ||--o{ USER_STREAM : joins
    STREAM ||--o| VIDEO : produces
    USER ||--o{ REFRESH_TOKEN : owns
    USER ||--o{ SUBSCRIPTION : follows

    USER {
        uuid id
    }
    STREAM {
        uuid id
        uuid user_id
        string stream_key
        datetime start_time
        datetime end_time
    }
    CHAT_MESSAGE {
        uuid id
        uuid stream_id
        uuid user_id
        string message
    }
    VIDEO {
        uuid id
        uuid stream_id
        string hls_url
    }
```

This shared database is convenient for the monolith, but it is the main dependency that must be removed or isolated during microservice migration.

## Current Coupling Points

1. Every module depends on the same `StreamingDbContext`.
2. Domain entities contain navigation properties to entities from other domains.
3. The Web process hosts REST, SignalR, RTMP and HLS responsibilities.
4. `StreamService` coordinates database state and FFmpeg lifecycle in one use case.
5. `ChatService` depends directly on a SignalR notification implementation.
6. There is no message broker or Outbox mechanism yet.
7. HLS files are local to the application container, which limits horizontal scaling.

## Target Direction

The migration should be incremental. We should first document boundaries, then extract one service at a time.

```mermaid
flowchart LR
    Gateway[API Gateway]
    Identity[Identity .NET]
    Stream[Stream Management .NET]
    Media[Media Service Go]
    Chat[Chat Service .NET]
    Video[Video Catalog .NET]
    Broker[(Message Broker)]

    Gateway --> Identity
    Gateway --> Stream
    Gateway --> Chat
    Gateway --> Video
    Publisher[OBS / FFmpeg] -->|RTMP| Media
    Media -->|StreamStarted / StreamEnded| Broker
    Broker --> Stream
```

The Go service is proposed for the media boundary because RTMP ingest, connection lifecycle and concurrent network processing are its primary responsibilities. This is an architectural proposal, not an implementation change yet.

## Definition of Done for This Document

- [x] Current projects and layers are documented.
- [x] Runtime entry point is documented.
- [x] Main business modules are identified.
- [x] REST, RTMP/HLS and chat flows are visualized.
- [x] Shared database and coupling points are documented.
- [x] Initial future service boundaries are proposed.

## Next Step

Use this document as the input for issue [#80](https://github.com/emil720a1/StreamingProject/issues/80), where we define bounded contexts, data ownership and the first extraction boundary.
