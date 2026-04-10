using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StreamingProject.Domain.Chat;
using StreamingProject.Domain.Permission;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.Stream.UserStream;
using StreamingProject.Domain.Subscription;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;
using StreamingProject.Domain.Video;
using StreamingProject.Repository.Configuration;

namespace StreamingProject.Repository;

public class StreamingDbContext : IdentityDbContext<UserEntity, RoleEntity, Guid, IdentityUserClaim<Guid>, UserRoleEntity, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly IOptions<AuthorizationOptions> _authOptions;
    public StreamingDbContext(DbContextOptions<StreamingDbContext> options, IOptions<AuthorizationOptions> authOptions) : base(options)
    {
        _authOptions = authOptions;
    }
    
    public DbSet<VideoEntity> Videos { get; set; }

    public DbSet<StreamEntity> Streams { get; set; }
    
    public DbSet<SubscriptionEntity> Subscriptions { get; set; }
    
    public DbSet<ChatEntity> ChatMessages { get; set; }
    
    public DbSet<PermissionEntity> Permissions { get; set; }
    
    public DbSet<UserStream> UserStreams { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StreamingDbContext).Assembly);
        
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(_authOptions.Value));
    }
}