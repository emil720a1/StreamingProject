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
                s.StreamKey,
                s.StartTime,
                s.EndTime
            ));
    }
}
