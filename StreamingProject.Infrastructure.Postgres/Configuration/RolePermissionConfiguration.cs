using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Configuration;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermissionEntity>
{
    private readonly AuthorizationOptions _authorizationOptions;

    private static readonly Guid AdminRoleId = new Guid("00000000-0000-0000-0000-000000000001");
    private static readonly Guid UserRoleId = new Guid("00000000-0000-0000-0000-000000000002");

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
            .SelectMany(rp => {
                var roleId = Enum.Parse<RoleEnum>(rp.Role) == RoleEnum.Admin ? AdminRoleId : UserRoleId;
                return rp.Permissions.Select(p => new RolePermissionEntity(
                    roleId,
                    (int)Enum.Parse<PermissionEnum>(p)
                ));
            })
            .ToArray();
    }
}