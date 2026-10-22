using StreamingProject.LoadTests;

try
{
    var options = LoadTestOptions.Parse(args);

    Console.WriteLine("RTMP load test configuration:");
    Console.WriteLine($"Endpoint: {options.Endpoint}");
    Console.WriteLine($"Stream key: {options.StreamKey}");
    Console.WriteLine($"Duration: {options.DurationSeconds} seconds");
    Console.WriteLine($"Clients: {options.Clients}");
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine($"Configuration error: {exception.Message}");
    return 1;
}

return 0;