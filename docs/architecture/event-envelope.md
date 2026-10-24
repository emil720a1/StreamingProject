# Event Envelope and Domain Events

> Issue #82 — define the common event contract for communication between services.

## Purpose

This document defines how microservices publish and consume domain events through the Message Broker.

The goal is to make events understandable, traceable, versioned, and safe to process repeatedly. The document defines a contract only; it does not implement Kafka, RabbitMQ, or the Outbox pattern.

## Command, query, and event

| Type | Meaning | Example |
|---|---|---|
| Command | request to perform an action | StartStream |
| Query | request to read data | GetStream |
| Event | fact that has already happened | stream.started |

An event must describe a completed fact, not an instruction for another service. A command may fail; an event is published only after the producer has committed the fact it represents.

~~~mermaid
flowchart LR
    C[Command] --> P[Producer service]
    P --> DB[(Owned database)]
    P --> O[Outbox]
    O -. publish .-> B[(Message Broker)]
    B -. deliver .-> S1[Consumer A]
    B -. deliver .-> S2[Consumer B]
~~~

## Event naming

Events use the following convention:

~~~text
<entity>.<past-tense-action>
~~~

Examples:

~~~text
stream.created
stream.started
stream.ended
user.created
subscription.created
chat.message.sent
video.published
~~~

The name uses a past-tense action because it represents a fact. Avoid command-like names such as start.stream, start_stream, or startStream.

## Initial event catalog

| Event type | Producer | Consumers | Meaning |
|---|---|---|---|
| user.created | Identity | Subscription, Chat, Catalog | a user was created |
| stream.created | Stream Management | Media, Catalog | business stream metadata was created |
| stream.started | Media | Stream Management, Catalog, Analytics | Media accepted RTMP and started processing |
| stream.ended | Media | Stream Management, Catalog, Analytics | Media stopped processing |
| subscription.created | Subscription | Stream Management | a subscription became active |
| chat.message.sent | Chat | moderation, notifications | a chat message was persisted and sent |
| video.published | Video Catalog | notifications, recommendations | a video became available for viewing |

The first implementation scope focuses on stream.started and stream.ended, because they connect the Go Media Service with Stream Management and the Video Catalog.

## Event Envelope

Every event uses the same outer structure:

~~~json
{
  "eventId": "01J...",
  "eventType": "stream.started",
  "eventVersion": 1,
  "occurredAt": "2026-10-24T10:00:00Z",
  "producer": "media-service",
  "correlationId": "operation-123",
  "causationId": "message-456",
  "payload": {
    "streamId": "stream-123",
    "mediaSessionId": "session-456",
    "startedAt": "2026-10-24T10:00:00Z"
  }
}
~~~

### Envelope fields

| Field | Required | Meaning |
|---|---:|---|
| eventId | yes | globally unique identifier of this event instance |
| eventType | yes | stable event name such as stream.started |
| eventVersion | yes | version of the event payload contract |
| occurredAt | yes | time when the business fact occurred, in UTC |
| producer | yes | logical service name that published the event |
| correlationId | yes | identifier connecting related requests, commands, and events |
| causationId | recommended | identifier of the message that caused this event |
| payload | yes | event-specific business data |

eventId is used for idempotency. correlationId is used for tracing one business operation. causationId explains the immediate cause of a particular event.

## Streaming event payloads

### stream.started

Producer: Media Service.

The event is published only after Media has accepted the RTMP publisher and started processing the media session.

~~~json
{
  "streamId": "stream-123",
  "mediaSessionId": "session-456",
  "startedAt": "2026-10-24T10:00:00Z"
}
~~~

Consumers may use this event to:

- change Stream Management status to Live;
- start recording metadata in Video Catalog;
- update analytics;
- notify interested clients.

### stream.ended

Producer: Media Service.

The event is published after the media session has stopped.

~~~json
{
  "streamId": "stream-123",
  "mediaSessionId": "session-456",
  "endedAt": "2026-10-24T10:30:00Z",
  "durationSeconds": 1800,
  "reason": "publisher_disconnected"
}
~~~

Consumers may use this event to:

- change Stream Management status to Ended;
- finalize recording metadata;
- calculate duration metrics;
- notify viewers and the stream owner.

## Event flow for a live stream

~~~mermaid
sequenceDiagram
    participant P as OBS / FFmpeg
    participant M as Media Service Go
    participant S as Stream Management
    participant O as Outbox
    participant B as Message Broker
    participant C as Video Catalog

    P->>M: RTMP publish with streamKey
    M->>S: validate stream key
    S-->>M: streamId + valid
    M->>M: start media session
    M->>O: store stream.started
    O->>B: publish event
    B-->>S: consume stream.started
    B-->>C: consume stream.started
    S->>S: status = Live
    P-->>M: publisher disconnects
    M->>O: store stream.ended
    O->>B: publish event
    B-->>S: consume stream.ended
    B-->>C: consume stream.ended
    S->>S: status = Ended
~~~

## Security and data minimization

The following values must not be included in events:

- streamKey;
- passwords or password hashes;
- refresh tokens or access tokens;
- payment card data;
- internal database connection details;
- raw FFmpeg command lines if they contain credentials.

Events should contain identifiers and business facts, not private credentials or infrastructure secrets.

## Versioning rules

- Events are immutable after publication.
- A compatible change adds an optional field and keeps existing fields valid.
- A breaking change creates a new event version.
- Consumers must ignore unknown fields.
- Consumers must support the event versions they explicitly subscribe to.
- The producer must not silently change the meaning of an existing field.

Example:

~~~text
stream.started v1 -> initial contract
stream.started v2 -> breaking payload change
~~~

The event type remains stable; eventVersion identifies the payload contract.

## Delivery and idempotency rules

Message delivery is at-least-once. A consumer may receive the same event more than once.

Each consumer must:

1. read eventId;
2. check whether that event was already processed;
3. skip the business action if it was processed before;
4. persist the processing result atomically with its local state when possible.

The consumer must not assume that events arrive in perfect order. If ordering is required for one stream, the broker key should be based on streamId.

## Reliability boundary

The producer writes its local business change and the Outbox record in one local database transaction. A background publisher later sends the Outbox record to the Message Broker.

This prevents the following failure:

~~~text
database commit succeeds
broker publish fails
event is lost
~~~

The detailed Outbox implementation is a separate task (#84). This document only defines the event contract that the Outbox will carry.

## Definition of Done

- [x] Event naming convention is defined.
- [x] Common Event Envelope is defined.
- [x] stream.started payload is defined.
- [x] stream.ended payload is defined.
- [x] Producers and consumers are listed.
- [x] Security rules are documented.
- [x] Versioning and idempotency rules are documented.
- [ ] Event contract classes are implemented.
- [ ] Outbox publication is implemented in issue #84.

## Next step

Implement the shared event contract classes and tests, then design the Outbox flow in Issue #84.
