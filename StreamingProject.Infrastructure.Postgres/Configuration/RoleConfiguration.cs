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

        var roles = Enum
            .GetValues<RoleEnum>()
            .Select(r => RoleEntity.Create((int)r, r.ToString()));
        
        builder.HasData(roles);

    }
}