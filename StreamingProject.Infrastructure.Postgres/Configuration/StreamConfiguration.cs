using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Stream;

namespace StreamingProject.Repository.Configuration;

public class StreamConfiguration : IEntityTypeConfiguration<StreamEntity>
{
    public void Configure(EntityTypeBuilder<StreamEntity> builder)
    {
        builder.ToTable("Streams");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.StreamKey)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasIndex(x => x.StreamKey)
            .IsUnique();
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Streams)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(a => a.ChatMessages)
            .WithOne(b => b.Stream)
            .HasForeignKey(x => x.StreamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Participants)
            .WithOne(p => p.Stream)
            .HasForeignKey(p => p.StreamId);
    }
    
}