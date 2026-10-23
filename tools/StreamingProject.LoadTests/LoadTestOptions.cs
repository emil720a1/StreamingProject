namespace StreamingProject.LoadTests;

public sealed record LoadTestOptions(
    string Endpoint,
    IReadOnlyList<string> StreamKeys,
    int DurationSeconds,
    int Clients)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var endpoint = "rtmp://backend:1935/live";
        var streamKeys = Array.Empty<string>();
        var durationSeconds = 30;
        var clients = 1;

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--endpoint":
                    endpoint = args[++index];
                    break;

                case "--stream-keys":
                case "--stream-key":
                    streamKeys = args[++index]
                        .Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries |
                            StringSplitOptions.TrimEntries);
                    break;

                case "--duration":
                    durationSeconds = int.Parse(args[++index]);
                    break;

                case "--clients":
                    clients = int.Parse(args[++index]);
                    break;

                default:
                    throw new ArgumentException(
                        $"Unknown argument: {args[index]}");
            }
        }

        if (streamKeys.Length == 0)
        {
            throw new ArgumentException(
                "The --stream-keys argument is required.");
        }

        if (durationSeconds <= 0)
        {
            throw new ArgumentException(
                "Duration must be greater than zero.");
        }

        if (clients <= 0)
        {
            throw new ArgumentException(
                "Clients must be greater than zero.");
        }

        if (clients > streamKeys.Length)
        {
            throw new ArgumentException(
                "The number of clients cannot exceed the number of stream keys.");
        }

        return new LoadTestOptions(
            endpoint,
            streamKeys,
            durationSeconds,
            clients);
    }
}
