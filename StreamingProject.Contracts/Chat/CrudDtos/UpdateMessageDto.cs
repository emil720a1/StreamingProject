namespace StreamingProject.Contracts.Chat.CrudDtos;

public record UpdateMessageDto(Guid MessageId, string NewText);