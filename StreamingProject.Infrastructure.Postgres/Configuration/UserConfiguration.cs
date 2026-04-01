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
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);

        builder.HasMany(u => u.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);
        
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.Username).IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.HasMany(u => u.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.Streams)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);
    }
}