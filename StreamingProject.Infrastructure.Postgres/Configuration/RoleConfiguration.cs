using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Permission;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Configuration;

public class RoleConfiguration : IEntityTypeConfiguration<RoleEntity>
{
    private static readonly Guid AdminRoleId = new Guid("00000000-0000-0000-0000-000000000001");
    private static readonly Guid UserRoleId = new Guid("00000000-0000-0000-0000-000000000002");

    public void Configure(EntityTypeBuilder<RoleEntity> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(x => x.Name).IsUnique();

        builder.HasMany(r => r.Permissions)
            .WithMany(p => p.Roles)
            .UsingEntity<RolePermissionEntity>(
                j => 
                    j.HasOne(rp => rp.Permission)
                    .WithMany()
                    .HasForeignKey(rp => rp.PermissionId),
                j =>
                    j.HasOne(rp => rp.Role)
                        .WithMany()
                        .HasForeignKey(rp => rp.RoleId),
                j =>
                {
                    j.ToTable("RolePermissions");
                    j.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                });

        builder.HasData(
            RoleEntity.Create(AdminRoleId, RoleEnum.Admin.ToString()),
            RoleEntity.Create(UserRoleId, RoleEnum.User.ToString())
        );
    }
}