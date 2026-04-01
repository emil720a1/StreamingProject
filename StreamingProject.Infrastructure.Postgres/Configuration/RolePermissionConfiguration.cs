using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Configuration;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{

    private readonly AuthorizationOptions _authorizationOptions;

    public RolePermissionConfiguration(AuthorizationOptions authorization)
    {
        _authorizationOptions = authorization;
    }
    
    
    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(r => new { r.RoleId, r.PermissionId });

        builder.HasData(ParseRolePermissions());
    }

    private RolePermissionEntity[] ParseRolePermissions()
    {
        return _authorizationOptions.RolePermissions
            .SelectMany(rp => rp.Permissions
                .Select(p => new RolePermissionEntity
                {
                    RoleId = (int)Enum.Parse<RoleEnum>(rp.Role),
                    PermissionId = (int)Enum.Parse<PermissionEnum>(p)
                }))
                 .ToArray();
    }
}