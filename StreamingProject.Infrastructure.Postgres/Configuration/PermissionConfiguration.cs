using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.ToTable("Permissions");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.Name).IsUnique();

        var permissions = Enum.GetValues<PermissionEnum>()
            .Select(p => PermissionEntity.Create((int)p, p.ToString()));
        
        builder.HasData(permissions);
    }
}