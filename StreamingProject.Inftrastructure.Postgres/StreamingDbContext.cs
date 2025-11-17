using Microsoft.EntityFrameworkCore;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Repository.Configuration;

namespace StreamingProject.Repository;

public class StreamingDbContext : DbContext
{

    public StreamingDbContext(DbContextOptions<StreamingDbContext> options) : base(options)
    {
    }
    

    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<VideoEntity> Videos { get; set; }


    public DbSet<StreamEntity> Streams { get; set; }
    
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    
    public DbSet<ChatEntity> ChatMessages { get; set; }
    
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VideoConfiguration());
        modelBuilder.ApplyConfiguration(new StreamConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ChatConfiguration());
        
        
        base.OnModelCreating(modelBuilder);
    }
}