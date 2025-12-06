using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Permission.PermissionRepository;
using StreamingProject.Domain.Permission;

namespace StreamingProject.Repository.Repositories.PermissionRepositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly StreamingDbContext _dbContext;

    public PermissionRepository(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task<List<PermissionEntity>> GetPermissionsAsync(int userId)
    {
        return await _dbContext.Permissions
            .Where(a => a.Id == userId)
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

    public Task<bool> RemovePermissionAsync(int userId, PermissionEntity permission)
    {
        throw new NotImplementedException();
    }
}