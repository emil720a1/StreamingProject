namespace StreamingProject.Contracts.Chat;

public record ChatMessagesDto(Guid Id, Guid UserId, string Message, DateTime SentAt, Guid StreamId);
