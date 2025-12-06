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


            .ForMember(d => d.Id,
                opt => opt.MapFrom(s => s.Id)
            )

            .ForMember(d => d.UserId,
                opt => opt.MapFrom(s => s.UserId)
            )

            .ForMember(d => d.StartDate,
                opt => opt.MapFrom(s => DateTime.Now));

        CreateMap<StreamDetailsDto, StreamEntity>();
    }
}