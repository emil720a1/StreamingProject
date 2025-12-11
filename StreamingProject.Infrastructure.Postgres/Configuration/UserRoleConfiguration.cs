using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
    public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });
        
        builder.HasOne(a => a.User)
            .WithMany(b => b.UserRoles)
            .HasForeignKey(a => a.UserId);
        
        builder.HasOne(a => a.Role)
            .WithMany(b => b.UserRoles)
            .HasForeignKey(a => a.RoleId);
    }
}