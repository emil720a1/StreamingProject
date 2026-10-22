namespace StreamingProject.LoadTests;

public sealed record LoadTestOptions(
    string Endpoint,
    string StreamKey,
    int DurationSeconds,
    int Clients)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var endpoint = "rtmp://backend:1935/live";
        var streamKey = string.Empty;
        var durationSeconds = 30;
        var clients = 1;

        for (var index = 0; index < args.Length; index++)
        {
            switch (args[index])
            {
                case "--endpoint":
                    endpoint = args[++index];
                    break;

                case "--stream-key":
                    streamKey = args[++index];
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

        if (string.IsNullOrWhiteSpace(streamKey))
        {
            throw new ArgumentException(
                "The --stream-key argument is required.");
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

        return new LoadTestOptions(
            endpoint,
            streamKey,
            durationSeconds,
            clients);
    }
}