SELECT
    streams.id,
    streams.title,
    users.username
FROM streams
JOIN users
    ON users.id = streams.user_id;
WHERE streams.end_time IS NULL;