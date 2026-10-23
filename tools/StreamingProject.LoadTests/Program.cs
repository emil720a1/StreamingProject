using StreamingProject.LoadTests;

try
{
    var options = LoadTestOptions.Parse(args);

    Console.WriteLine("RTMP load test configuration:");
    Console.WriteLine($"Endpoint: {options.Endpoint}");
    Console.WriteLine(
        $"Stream keys: {string.Join(", ", options.StreamKeys)}");
    Console.WriteLine($"Duration: {options.DurationSeconds} seconds");
    Console.WriteLine($"Clients: {options.Clients}");

    var runner = new RtmpClientRunner();

    var tasks = options.StreamKeys
        .Take(options.Clients)
        .Select((streamKey, index) =>
            RunClientAsync(runner, options, streamKey, index + 1));

    var exitCodes = await Task.WhenAll(tasks);

    return exitCodes.All(code => code == 0) ? 0 : 1;
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Configuration error: {exception.Message}");
    return 1;
}

static async Task<int> RunClientAsync(
    RtmpClientRunner runner,
    LoadTestOptions options,
    string streamKey,
    int clientNumber)
{
    Console.WriteLine(
        $"[client {clientNumber}] Starting with stream key {streamKey}.");

    try
    {
        var exitCode = await runner.RunAsync(
            options,
            streamKey,
            clientNumber);

        Console.WriteLine(
            $"[client {clientNumber}] Finished with exit code {exitCode}.");

        return exitCode;
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine(
            $"[client {clientNumber}] Failed: {exception.Message}");

        return 1;
    }
}
