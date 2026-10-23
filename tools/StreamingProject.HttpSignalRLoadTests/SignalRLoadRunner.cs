using Microsoft.AspNetCore.SignalR.Client;

namespace StreamingProject.HttpSignalRLoadTests;

public sealed class SignalRLoadRunner
{
    public async Task RunAsync(
        LoadTestOptions options,
        int clientNumber,
        CancellationToken cancellationToken = default)
    {
        var hubUri = $"{options.Endpoint}{options.HubPath}";
        await using var connection = new HubConnectionBuilder()
            .WithUrl(hubUri)
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync(cancellationToken);
        await connection.InvokeAsync(
            "JoinStreamGroup",
            options.StreamId,
            cancellationToken);

        Console.WriteLine(
            $"[signalr client {clientNumber}] connected and joined {options.StreamId}");

        try
        {
            await Task.Delay(
                TimeSpan.FromSeconds(options.DurationSeconds),
                cancellationToken);
        }
        finally
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.InvokeAsync(
                    "LeaveStreamGroup",
                    options.StreamId,
                    cancellationToken);
            }

            await connection.StopAsync(CancellationToken.None);
        }

        Console.WriteLine(
            $"[signalr client {clientNumber}] disconnected successfully");
    }
}
