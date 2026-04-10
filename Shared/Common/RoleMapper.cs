using AutoMapper;
using Microsoft.AspNetCore.Identity;
using StreamingProject.Contracts.Roles;
using StreamingProject.Contracts.Roles.RoleDetailsDto;
using StreamingProject.Domain.Permission;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace Shared.Common;

public class RoleMapper : Profile
{
 public RoleMapper()
 {
     CreateMap<RoleEntity, RoleDetailsDto>()
         .ForMember(a => a.Permissions, opt => opt.MapFrom(src => 
             (src.Permissions ?? new List<PermissionEntity>())
             .Select(p => p.Name).ToList()));
 }   
}