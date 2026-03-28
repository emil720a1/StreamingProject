using AutoMapper;
using StreamingProject.Contracts.Streams;
using StreamingProject.Domain;
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
                s.StartTime
            ));
    }
}
