namespace StreamingProject.Contracts.VideoDto;

public record VideoDetailsDto(Guid Id, Guid StreamId, string FileUrl, string HlsUrl);