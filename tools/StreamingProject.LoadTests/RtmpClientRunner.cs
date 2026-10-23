using System.Diagnostics;
using System.Globalization;

namespace StreamingProject.LoadTests;

public sealed class RtmpClientRunner
{
    public async Task<int> RunAsync(
        LoadTestOptions options,
        string streamKey,
        int clientNumber,
        CancellationToken cancellationToken = default)
    {
        var streamUrl =
            $"{options.Endpoint.TrimEnd('/')}/{streamKey}";

        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            UseShellExecute = false,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        startInfo.ArgumentList.Add("-re");

        startInfo.ArgumentList.Add("-f");
        startInfo.ArgumentList.Add("lavfi");

        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("testsrc=size=640x360:rate=15");

        startInfo.ArgumentList.Add("-f");
        startInfo.ArgumentList.Add("lavfi");

        startInfo.ArgumentList.Add("-i");
        startInfo.ArgumentList.Add("sine=frequency=1000:sample_rate=44100");

        startInfo.ArgumentList.Add("-t");
        startInfo.ArgumentList.Add(
            options.DurationSeconds.ToString(CultureInfo.InvariantCulture));

        startInfo.ArgumentList.Add("-c:v");
        startInfo.ArgumentList.Add("libx264");

        startInfo.ArgumentList.Add("-preset");
        startInfo.ArgumentList.Add("ultrafast");

        startInfo.ArgumentList.Add("-tune");
        startInfo.ArgumentList.Add("zerolatency");

        startInfo.ArgumentList.Add("-pix_fmt");
        startInfo.ArgumentList.Add("yuv420p");

        startInfo.ArgumentList.Add("-c:a");
        startInfo.ArgumentList.Add("aac");

        startInfo.ArgumentList.Add("-f");
        startInfo.ArgumentList.Add("flv");

        startInfo.ArgumentList.Add(streamUrl);

        using var process = new Process
        {
            StartInfo = startInfo,
        };

        process.ErrorDataReceived += (_, eventArgs) =>
        {
            if (!string.IsNullOrEmpty(eventArgs.Data))
            {
                Console.WriteLine(
                    $"[client {clientNumber}] [ffmpeg] {eventArgs.Data}");
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start FFmpeg process.");
        }

        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }

            throw;
        }
        return process.ExitCode;
    }
}
