namespace StreamingProject.Contracts.Chat;

public record SendMessageDto(Guid StreamId, string Message);