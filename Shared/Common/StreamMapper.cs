using AutoMapper;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain.Stream;

namespace Shared.Common;

public class StreamMapper : Profile
{
    public StreamMapper()
    {
        CreateMap<StreamEntity, StreamDetailsDto>()
            .ConstructUsing(s => new StreamDetailsDto(
                s.Id,
                s.UserId,
                s.User != null ? s.User.UserName : null,
                s.Title,
                s.Description,
                s.StartTime,
                s.EndTime
            ));

        CreateMap<StreamEntity, CreateStreamResponseDto>()
            .ConstructUsing(s => new CreateStreamResponseDto(
                s.Id,
                s.StreamKey
            ));


        CreateMap<StreamEntity, StreamListItemDto>()
            .ConstructUsing(s => new StreamListItemDto(
                s.Id,
                s.UserId,
                s.User != null ? s.User.UserName : null,
                s.Title,
                s.Description,
                s.StartTime
                ));
    }
}
