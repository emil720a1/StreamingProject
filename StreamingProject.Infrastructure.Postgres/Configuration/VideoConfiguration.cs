using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Video;

namespace StreamingProject.Repository.Configuration;

public class VideoConfiguration : IEntityTypeConfiguration<VideoEntity>
{
    public void Configure(EntityTypeBuilder<VideoEntity> builder)
    {
        builder.ToTable("Videos");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.HlsUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .HasOne(a => a.Stream)
            .WithOne(b => b.VideoEntity)
            .HasForeignKey<VideoEntity>(a => a.StreamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.StreamId).IsUnique();
    }
}