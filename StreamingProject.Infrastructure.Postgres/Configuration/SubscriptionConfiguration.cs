using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StreamingProject.Domain;
using StreamingProject.Domain.Subscription;

namespace StreamingProject.Repository.Configuration;

public class SubscriptionConfiguration : IEntityTypeConfiguration<SubscriptionEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionEntity> builder)
    {
        builder.ToTable("Subscriptions");
        
        builder.HasKey(x => new {x.FollowerId, x.FollowedId});

        builder.HasOne(x => x.Follower)
            .WithMany(u => u.Followings)
            .HasForeignKey(x => x.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(x => x.FollowedId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.SubscriptionAt)
            .IsRequired();
    }
}