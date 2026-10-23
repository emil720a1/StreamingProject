namespace StreamingProject.HttpSignalRLoadTests;

public sealed record LoadTestOptions(
    string Endpoint,
    string Mode,
    string HttpPath,
    string HubPath,
    string StreamId,
    int DurationSeconds,
    int Clients,
    int IntervalMilliseconds)
{
    public static LoadTestOptions Parse(string[] args)
    {
        var endpoint = "http://backend:8080";
        var mode = "all";
        var httpPath = "/health";
        var hubPath = "/chatHub";
        var streamId = "load-test";
        var durationSeconds = 30;
        var clients = 1;
        var intervalMilliseconds = 100;

        for (var index = 0; index < args.Length; index++)
        {
            if (index + 1 >= args.Length)
            {
                throw new ArgumentException($"Value is missing for {args[index]}.");
            }

            switch (args[index])
            {
                case "--endpoint":
                    endpoint = args[++index];
                    break;
                case "--mode":
                    mode = args[++index].ToLowerInvariant();
                    break;
                case "--http-path":
                    httpPath = args[++index];
                    break;
                case "--hub-path":
                    hubPath = args[++index];
                    break;
                case "--stream-id":
                    streamId = args[++index];
                    break;
                case "--duration":
                    durationSeconds = ParsePositiveInt(args[++index], "duration");
                    break;
                case "--clients":
                    clients = ParsePositiveInt(args[++index], "clients");
                    break;
                case "--interval-ms":
                    intervalMilliseconds = ParseNonNegativeInt(args[++index], "interval-ms");
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[index]}");
            }
        }

        if (mode is not ("http" or "signalr" or "all"))
        {
            throw new ArgumentException("Mode must be http, signalr, or all.");
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var endpointUri)
            || endpointUri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("Endpoint must be an absolute HTTP or HTTPS URL.");
        }

        return new LoadTestOptions(
            endpoint.TrimEnd('/'),
            mode,
            EnsurePath(httpPath, "http-path"),
            EnsurePath(hubPath, "hub-path"),
            streamId,
            durationSeconds,
            clients,
            intervalMilliseconds);
    }

    private static string EnsurePath(string value, string optionName)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith('/'))
        {
            throw new ArgumentException($"The --{optionName} value must start with '/'.");
        }

        return value;
    }

    private static int ParsePositiveInt(string value, string optionName)
    {
        if (!int.TryParse(value, out var result) || result <= 0)
        {
            throw new ArgumentException($"The --{optionName} value must be greater than zero.");
        }

        return result;
    }

    private static int ParseNonNegativeInt(string value, string optionName)
    {
        if (!int.TryParse(value, out var result) || result < 0)
        {
            throw new ArgumentException($"The --{optionName} value cannot be negative.");
        }

        return result;
    }
}
