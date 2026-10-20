using System.Diagnostics;

namespace StreamingProject.Application.Observability;

public static class StreamingActivitySource
{
    public const string Name = "StreamingProject.Application";

    public static readonly ActivitySource Source = new(Name);
}