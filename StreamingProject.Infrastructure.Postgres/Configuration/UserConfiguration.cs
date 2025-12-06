using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        
        builder.HasKey(x => x.Id);

        builder.HasMany(u => u.Roles)
            .WithMany(r => r.Users)
            .UsingEntity<UserRoleEntity>(
                l => l.HasOne<RoleEntity>().WithMany().HasForeignKey(r => r.RoleId),
                 r => r.HasOne<UserEntity>().WithMany().HasForeignKey(u => u.UserId));
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(x => x.Username).IsUnique();
        
        
        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(64);


    }
}