namespace StreamingProject.Contracts.VideoDto.Crud;

public record CreateVideoDto(string Title, Guid StreamId, string FileUrl, string HlsUrl);