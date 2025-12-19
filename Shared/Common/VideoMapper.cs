using AutoMapper;
using StreamingProject.Contracts.VideoDto;
using StreamingProject.Domain.Video;

namespace Shared.Common;

public class VideoMapper : Profile
{
    public VideoMapper()
    {
        CreateMap<VideoEntity, VideoDetailsDto>()

            .ConstructUsing(s => new VideoDetailsDto(
                s.Id,
                s.StreamId,
                s.FileUrl,
                s.HlsUrl
                ));
    }
}