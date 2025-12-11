using AutoMapper;
using StreamingProject.Contracts.Permissions.PermissionDetailsDto;
using StreamingProject.Domain.Permission;

namespace Shared.Common;

public class PermissionMapper : Profile
{
    public PermissionMapper()
    {
        CreateMap<PermissionEntity, PermissionDetailsDto>()

            .ConstructUsing(s => new PermissionDetailsDto(s.Id, s.Name));


    }
}