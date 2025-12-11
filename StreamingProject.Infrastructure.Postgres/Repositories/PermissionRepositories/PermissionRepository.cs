using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Permission.PermissionRepository;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Repositories.PermissionRepositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly StreamingDbContext _dbContext;

    public PermissionRepository(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<List<PermissionEntity>> GetPermissionsAsync(Guid userId)
    {
        return await _dbContext.UserRoles
            .Where(a => a.UserId == userId)
            .SelectMany(ur => ur.Role.Permissions)
            .Distinct()
            .ToListAsync();
    }

    public async Task<PermissionEntity?> GetPermissionAsync(int userId)
    {
        return await _dbContext.Permissions
            .FirstOrDefaultAsync(a => a.Id == userId);
    }


    public async Task<PermissionEntity> AddPermissionAsync(int userId, PermissionEntity permissions)
    {
        
        await _dbContext.Permissions
            .AddRangeAsync(permissions);
        await _dbContext.SaveChangesAsync();
        
        return permissions;
    }

    public async Task<bool> RemovePermissionAsync(int userId, List<PermissionEntity> permissions)
    {
      
        _dbContext.Permissions.RemoveRange(permissions);
        await _dbContext.SaveChangesAsync();

        return true;
    }
    

    public async Task<PermissionEntity> UpdatePermissionAsync(int userId, PermissionEntity permissions)
    {
        _dbContext.Permissions.UpdateRange(permissions);
        await _dbContext.SaveChangesAsync();

        return permissions;

    }

    public async Task<bool> RemovePermissionAsync(int userId, PermissionEnum permission)
    {
        int permissionId = (int)permission;

        var userPermission = await _dbContext.Permissions
            .FirstOrDefaultAsync(a => a.Id == userId);

        if (userPermission == null)
        {
            return false;
        }
        
        _dbContext.Permissions.Remove(userPermission);
        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }
    
}