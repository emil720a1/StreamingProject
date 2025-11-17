using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;

namespace StreamingProject.Repository.Configuration;

public class StreamConfiguration : IEntityTypeConfiguration<StreamEntity>
{
    public void Configure(EntityTypeBuilder<StreamEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne(x => x.User)
            .WithMany(x => x.Streams)
            .HasForeignKey(x => x.UserId);

        builder
            .HasMany(a => a.ChatMessages)
            .WithOne(b => b.Stream)
            .HasForeignKey(x => x.Id)
            .IsRequired();
    }
    
}