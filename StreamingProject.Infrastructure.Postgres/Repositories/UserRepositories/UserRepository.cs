using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Domain;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Permission;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Repositories.UserRepositories;

public class UserRepository : IUserRepository
{
    private readonly StreamingDbContext _dbContext;

    public UserRepository(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserEntity> AddUserAsync(UserEntity user)
    {

        if (user.UserRoles != null)
        {
            foreach (var userRole in user.UserRoles)
            {
                if (userRole.Role != null)
                {
                    _dbContext.Entry(userRole.Role).State = EntityState.Unchanged;
                }
            }
        }
        
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<UserEntity> UpdateUserAsync(UserEntity user)
    {
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var deletedCount = await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
        
        return deletedCount > 0;
    }

    public async Task<UserEntity?> GetUserById(Guid id)
    {
        return await _dbContext.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<UserEntity?> GetStreamsByUserId(Guid userId)
    {
        return await _dbContext.Users
            .Include(a => a.Streams)
            .FirstOrDefaultAsync(a => a.Id == userId);
    }

    public async Task<bool> UserExists(string username, string email)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.Username == username || u.Email == email);
    }
    

    public async Task<UserEntity?> GetByEmail(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<HashSet<PermissionEnum>> GetUserPermissions(Guid userId)
    {
        var permissions = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.Permissions)
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles)
                .Select(ur => ur.Role)
                .SelectMany(r => r.Permissions)
                .Select(p => (PermissionEnum)p.Id)
                .ToListAsync();
        
        return permissions.ToHashSet();
    }
}

    