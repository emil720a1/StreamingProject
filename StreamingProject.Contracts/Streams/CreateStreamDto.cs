using StreamingProject.Domain;

namespace StreamingProject.Contracts.Streams;

public record CreateStreamDto(Guid UserId , Guid Id, DateTime StartDate);