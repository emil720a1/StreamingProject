using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain.Stream.UserStream;

namespace StreamingProject.Repository.Configuration;

public class UserStreamConfiguration : IEntityTypeConfiguration<UserStream>
{
    public void Configure(EntityTypeBuilder<UserStream> builder)
    {
        builder.HasKey(x => new {x.StreamId, x.UserId});
        
    }
}