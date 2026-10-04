SELECT
    id,
    title,
    description,
    start_time,
    end_time
FROM streams
WHERE end_time IS NULL
ORDER BY start_time DESC;