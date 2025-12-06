using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Video;

namespace StreamingProject.Repository.Configuration;

public class VideoConfiguration : IEntityTypeConfiguration<VideoEntity>
{
    public void Configure(EntityTypeBuilder<VideoEntity> builder)
    {
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.FileUrl).IsRequired();
        builder.Property(x => x.HlsUrl).IsRequired();

        builder
            .HasOne(a => a.Stream)
            .WithOne(b => b.VideoEntity)
            .HasForeignKey<VideoEntity>(a => a.StreamId)
            .IsRequired();
    }
}