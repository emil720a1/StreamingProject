namespace Shared.Correlation;

public class CorrelationContext : ICorrelationContext
{
    public string? CorrelationId { get; set; }
}