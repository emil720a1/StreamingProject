using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Configuration;

public class PermissionConfiguration : IEntityTypeConfiguration<PermissionEntity>
{
    public void Configure(EntityTypeBuilder<PermissionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        
        var permissions = Enum
            .GetValues<PermissionEnum>()
            .Select(p => new PermissionEntity
        {
            Id = (int)p,
            Name = p.ToString()    
        });
        
        builder.HasData(permissions);
    }
}