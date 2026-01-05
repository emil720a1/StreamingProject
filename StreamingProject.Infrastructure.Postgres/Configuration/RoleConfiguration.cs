using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Permission;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<RolePermissionEntity>(
                l => 
                    l.HasOne(e => e.Permission)
                    .WithMany()
                    .HasForeignKey(e => e.PermissionId),
                r =>
                    r.HasOne(e => e.Role)
                        .WithMany()
                        .HasForeignKey(e => e.RoleId));

        var roles = Enum
            .GetValues<RoleEnum>()
            .Select(r => RoleEntity.Create((int)r, r.ToString()));
        
        builder.HasData(roles);

    }
}