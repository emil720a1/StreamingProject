using AutoMapper;
using StreamingProject.Contracts.User;
using StreamingProject.Domain.User;

namespace Shared.Common;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDetailsDto>()
            .ForMember(d => d.Id,
                opt => opt.MapFrom(s => s.Id)
            )

            .ForMember(d => d.Username,
                opt => opt.MapFrom(s => s.Username)
            )

            .ForMember(d => d.FirstName,
                opt => opt.MapFrom(s => s.FirstName)
            )

            .ForMember(d => d.LastName,
                opt => opt.MapFrom(s => s.LastName)
            )

            .ForMember(d => d.Roles,
                opt => 
                    opt.MapFrom(s => s.Roles.Select(ur => ur.Name))

            )

            .ForMember(d => d.Status,
                opt => opt.MapFrom(s => s.Status)
            );
    }
    
}