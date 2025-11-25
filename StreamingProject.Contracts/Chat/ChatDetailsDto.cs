namespace StreamingProject.Contracts.Chat;

public record ChatDetailsDto(Guid Id, string Message, DateTime SentAt, Guid UserId);

