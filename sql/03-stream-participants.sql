SELECT
    streams.title,
    users.username,
    user_streams.joined_at
FROM user_streams
JOIN users
    ON users.id = user_streams.user_id
JOIN streams
    ON streams.id = user_streams.stream_id;