using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;

namespace StreamingProject.Repository.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<ChatEntity>
{
    public void Configure(EntityTypeBuilder<ChatEntity> builder)
    {

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Message).IsRequired().HasMaxLength(500);
    }

   
}