using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StreamingProject.Application.Service.Permission.PermissionRepository;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Repositories.PermissionRepositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly ILogger<PermissionRepository> _logger;
    private readonly StreamingDbContext _dbContext;

    public PermissionRepository(StreamingDbContext dbContext, ILogger<PermissionRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    
    public async Task<List<PermissionEntity>> GetPermissionsAsync(Guid userId)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .SelectMany(u => u.UserRoles)
            .Select(ur => ur.Role)
            .SelectMany(r => r.Permissions)
            .Distinct()
            .ToListAsync();
    }


    public async Task<PermissionEntity> AddPermissionAsync(PermissionEntity permission)
    {
        await _dbContext.Permissions.AddAsync(permission);
        await _dbContext.SaveChangesAsync();
        
        return permission;
    }


    public async Task<bool> RemovePermissionAsync(Guid userId, int permissionId)
    {
        var permission = await _dbContext.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId);

        if (permission == null)
        {
            _logger.LogWarning($"User {userId} tried to delete non-existent permission {permissionId}", userId, permissionId);
            return false;
        }
        
        _dbContext.Permissions.Remove(permission);
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"Permission {permissionId} was deleted by user {userId}", permissionId, userId);
        return true;
    }
}