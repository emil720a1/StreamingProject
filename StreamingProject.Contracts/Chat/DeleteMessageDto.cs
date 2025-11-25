namespace StreamingProject.Contracts.Chat;

public record DeleteMessageDto(Guid Id, Guid StreamId, string Message, Guid UserId );