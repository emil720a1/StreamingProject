using System.Diagnostics.Metrics;

namespace StreamingProject.Application.Observability;

public sealed class StreamingMetrics
{
    public const string MeterName = "StreamingProject.Streaming";

    private readonly Counter<long> _streamsStarted;
    private readonly Counter<long> _streamsEnded;
    private readonly UpDownCounter<long> _activeStreams;
    private readonly Histogram<double> _streamDuration;

    public StreamingMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _streamsStarted =
            meter.CreateCounter<long>("streaming_streams_started_total");

        _streamsEnded =
            meter.CreateCounter<long>("streaming_streams_ended_total");

        _activeStreams =
            meter.CreateUpDownCounter<long>("streaming_active_streams");

        _streamDuration =
            meter.CreateHistogram<double>(
                "streaming_stream_duration_seconds",
                unit: "s");
    }

    public void StreamStarted()
    {
        _streamsStarted.Add(1);
        _activeStreams.Add(1);
    }

    public void StreamEnded(TimeSpan duration)
    {
        _streamsEnded.Add(1);
        _activeStreams.Add(-1);
        _streamDuration.Record(duration.TotalSeconds);
    }
}