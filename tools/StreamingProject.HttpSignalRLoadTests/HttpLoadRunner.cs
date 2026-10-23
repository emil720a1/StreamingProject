using System.Diagnostics;

namespace StreamingProject.HttpSignalRLoadTests;

public sealed class HttpLoadRunner
{
    private readonly HttpClient _httpClient = new();

    public async Task RunAsync(
        LoadTestOptions options,
        int clientNumber,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{options.Endpoint}{options.HttpPath}";
        var deadline = Stopwatch.GetTimestamp()
            + Stopwatch.Frequency * options.DurationSeconds;
        var successfulRequests = 0;
        var failedRequests = 0;

        while (Stopwatch.GetTimestamp() < deadline)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    requestUri,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    successfulRequests++;
                }
                else
                {
                    failedRequests++;
                }
            }
            catch (HttpRequestException)
            {
                failedRequests++;
            }

            if (options.IntervalMilliseconds > 0)
            {
                await Task.Delay(options.IntervalMilliseconds, cancellationToken);
            }
        }

        Console.WriteLine(
            $"[http client {clientNumber}] completed: "
            + $"success={successfulRequests}, failed={failedRequests}");
    }
}
