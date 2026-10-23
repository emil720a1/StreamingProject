using StreamingProject.HttpSignalRLoadTests;

try
{
    var options = LoadTestOptions.Parse(args);

    Console.WriteLine("HTTP/SignalR load test configuration:");
    Console.WriteLine($"Endpoint: {options.Endpoint}");
    Console.WriteLine($"Mode: {options.Mode}");
    Console.WriteLine($"Duration: {options.DurationSeconds} seconds");
    Console.WriteLine($"Clients: {options.Clients}");

    var tasks = new List<Task>();

    if (options.Mode is "http" or "all")
    {
        var httpRunner = new HttpLoadRunner();
        tasks.AddRange(Enumerable.Range(1, options.Clients)
            .Select(client => httpRunner.RunAsync(options, client)));
    }

    if (options.Mode is "signalr" or "all")
    {
        var signalRRunner = new SignalRLoadRunner();
        tasks.AddRange(Enumerable.Range(1, options.Clients)
            .Select(client => signalRRunner.RunAsync(options, client)));
    }

    await Task.WhenAll(tasks);
    return 0;
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Configuration error: {exception.Message}");
    return 1;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Load test failed: {exception.Message}");
    return 1;
}
