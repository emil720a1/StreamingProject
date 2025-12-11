using AutoMapper;
using StreamingProject.Contracts.User;
using StreamingProject.Domain.User;

namespace Shared.Common;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<UserEntity, UserDetailsDto>()

            .ConstructUsing(s => new UserDetailsDto(
                s.Id,
                s.Username,
                s.FirstName,
                s.LastName,
                s.UserRoles.Select(ur => ur.Role.Name).ToList(),
                s.Status
            ));
    }
    
}