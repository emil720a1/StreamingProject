using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Repository.Configuration;

public class UserStreamConfiguration : IEntityTypeConfiguration<UserStream>
{
    public void Configure(EntityTypeBuilder<UserStream> builder)
    {
        builder.ToTable("UserStreams");

        builder.HasKey(us => new { us.UserId, us.StreamId });
        
        builder.HasOne(us => us.User)
            .WithMany(u => u.JoinedStreams)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(us => us.Stream)
            .WithMany(s => s.Participants)
            .HasForeignKey(us => us.StreamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(us => us.JoinedAt)
            .IsRequired();
    }
}