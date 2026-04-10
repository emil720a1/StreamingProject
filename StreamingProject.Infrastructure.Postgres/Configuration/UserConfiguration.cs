using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.User;

namespace StreamingProject.Repository.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserName)
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.UserName).IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150);
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.PasswordHash)
            .IsRequired();
        
        builder.HasMany(u => u.UserRoles)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .IsRequired();
        
        builder.HasMany(u => u.Streams)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);
    }
}