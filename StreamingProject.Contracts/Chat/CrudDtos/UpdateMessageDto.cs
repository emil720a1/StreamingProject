namespace StreamingProject.Contracts.Chat;

public record UpdateMessageDto(Guid messageId, Guid streamId, Guid userId, string newText);