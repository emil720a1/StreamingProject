namespace StreamingProject.Contracts.VideoDto.Crud;

public record UpdateVideoDto(Guid Id, Guid userId, string Title);