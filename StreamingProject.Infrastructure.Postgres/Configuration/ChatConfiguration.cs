using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Chat;

namespace StreamingProject.Repository.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<ChatEntity>
{
    public void Configure(EntityTypeBuilder<ChatEntity> builder)
    {

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(1000);
        
        builder.Property(x => x.SentTime)
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Stream)
            .WithMany(s => s.ChatMessages)
            .HasForeignKey(x => x.StreamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(x => x.StreamId);
    }
}