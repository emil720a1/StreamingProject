namespace StreamingProject.Contracts.Chat.CrudDtos;

public record SendMessageDto(Guid StreamId, string Message);