namespace Shared.Correlation;

public interface ICorrelationContext
{
    string? CorrelationId { get; set; }
}