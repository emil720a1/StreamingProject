namespace StreamingProject.Contracts.Chat;

public record SendMessageDto(Guid Id,Guid StreamId, string Message, Guid UserId, DateTime SentAt);