# Project Architecture

## Overview

StreamingProject is a streaming platform built with ASP.NET Core, Entity Framework Core and PostgreSQL.

The system supports:

- user registration and authentication;
- JWT and refresh-token authentication;
- live streams;
- stream participants;
- SignalR chat;
- recorded videos and HLS playback;
- subscriptions;
- roles and permissions.

## Backend Projects

| Project | Responsibility |
|---|---|
| `StreamingProject.Domain` | Domain entities, enums and business rules |
| `StreamingProject.Contracts` | DTOs used by the API |
| `StreamingProject.Application` | Application services and use cases |
| `StreamingProject.Infrastructure.Postgres` | EF Core, PostgreSQL, repositories and infrastructure services |
| `StreamingProject.Presenters` | API controllers and SignalR hubs |
| `StreamingProject.Web` | Application startup, dependency injection and HTTP pipeline |
| `Shared` | Shared failures, exceptions, mappers and common utilities |

## Entity Relationships

### User and Stream

Relationship: one-to-many.

One user can create multiple streams. Each stream belongs to one user.

Foreign key:

- `StreamEntity.UserId` references `UserEntity.Id`.

Navigation properties:

- `UserEntity.Streams`;
- `StreamEntity.User`.

Deleting a user also deletes their streams because cascade delete is configured.

### Stream and Chat Messages

Relationship: one-to-many.

One stream contains multiple chat messages. Each chat message belongs to one stream.

Foreign key:

- `ChatEntity.StreamId` references `StreamEntity.Id`.

Navigation properties:

- `StreamEntity.ChatMessages`;
- `ChatEntity.Stream`.

A chat message also belongs to the user who sent it through `ChatEntity.UserId`.

Deleting a stream also deletes its chat messages.

### User and Chat Messages

Relationship: one-to-many.

One user can send multiple chat messages. Each message belongs to one user.

Foreign key:

- `ChatEntity.UserId` references `UserEntity.Id`.

Each message stores the message text, sender ID, stream ID and sent time.

### Stream and Video

Relationship: one-to-one.

A stream can have one recorded video. A video belongs to one stream.

Foreign key:

- `VideoEntity.StreamId` references `StreamEntity.Id`.

The `StreamId` column is unique, so one stream cannot have multiple video records.

A video stores the original file URL, HLS URL, title, stream ID, owner ID and creation date.

### Users and Stream Participants

Relationship: many-to-many.

Users can join multiple streams. A stream can have multiple participants.

The relationship is represented by the `UserStream` entity.

The linking entity contains:

- `UserId`;
- `StreamId`;
- `JoinedAt`.

The composite key is:

```text
UserId + StreamId
```

This prevents the same user from joining the same stream multiple times.

### User Subscriptions

Relationship: many-to-many between users.

A user can follow multiple users. A user can have multiple followers.

The relationship is represented by the `SubscriptionEntity`.

Fields:

- `FollowerId` — the user who follows;
- `FollowedId` — the user being followed;
- `SubscriptionAt` — subscription creation time.

The composite key is:

```text
FollowerId + FollowedId
```

This prevents duplicate subscriptions. A user cannot follow themselves.

### Users and Roles

Relationship: many-to-many.

A user can have multiple roles. A role can be assigned to multiple users.

The relationship is represented by the `UserRoleEntity`.

The linking entity contains `UserId` and `RoleId`.

Examples of roles:

- `User`;
- `Admin`.

### Roles and Permissions

Relationship: many-to-many.

A role can have multiple permissions. A permission can belong to multiple roles.

The relationship is represented by the `RolePermissionEntity`.

The linking entity contains `RoleId` and `PermissionId`.

### User and Refresh Tokens

Relationship: one-to-many.

One user can have multiple refresh tokens. Each refresh token belongs to one user.

A refresh token contains its value, user ID, expiration date and revoked status.

A refresh token is active only when it has not expired and has not been revoked.

### Users and Stream Likes

Relationship: many-to-many.

Users can like multiple streams. A stream can have multiple likes.

The relationship is represented by `StreamLikeEntity` and contains `UserId`, `StreamId` and `LikeTime`.

The project should prevent one user from liking the same stream more than once.

## Relationship Summary

```text
UserEntity 1 ──── * StreamEntity
UserEntity 1 ──── * ChatEntity
StreamEntity 1 ──── * ChatEntity
StreamEntity 1 ──── 0..1 VideoEntity
UserEntity * ──── * StreamEntity through UserStream
UserEntity * ──── * UserEntity through SubscriptionEntity
UserEntity * ──── * RoleEntity through UserRoleEntity
RoleEntity * ──── * PermissionEntity through RolePermissionEntity
UserEntity 1 ──── * RefreshToken
UserEntity * ──── * StreamEntity through StreamLikeEntity
```

## Known Architecture Issues

- `StreamLikeEntity` should have explicit EF Core relationship configuration.
- A unique constraint should prevent duplicate stream likes.
- Refresh-token relationship configuration should be verified.
- Cascade delete rules should be reviewed for production safety.
- Entity relationships should be covered by integration tests.
