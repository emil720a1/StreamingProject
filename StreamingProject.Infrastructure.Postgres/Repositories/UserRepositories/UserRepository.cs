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
        var roleEntity = await _dbContext.Roles
            .SingleOrDefaultAsync(a => a.Id == (int)RoleEnum.User)
            ?? throw new InvalidOperationException($"Role with ID {(int)RoleEnum.User} not found.");


        var userEntity = UserEntity.Create(
            Username: user.Username,
            Password: user.Password,
            Email: user.Email,
            Role: roleEntity
            );
        
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        return user;
    }


    public async Task<UserEntity> UpdateUserAsync(UserEntity user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var user = await GetUserById(id);
        if (String.IsNullOrEmpty(user?.Id.ToString())) return false;

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<UserEntity?> GetUserById(Guid id)
    {
        return await _dbContext.Users
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
            .FirstOrDefaultAsync(a => a.Username == username && a.Email == email) != null;
    }
    

    public async Task<UserEntity> GetByEmail(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email) ?? throw new Exception("User not found");
    }

    public async Task<HashSet<PermissionEnum>> GetUserPermissions(Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(a => a.UserRoles)
               .ThenInclude(ur => ur.Role)
                   .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == userId)
            .FirstOrDefaultAsync();
        
        if (user == null) return new HashSet<PermissionEnum>();

        return user.UserRoles
            .Select(ur => ur.Role)
            .Where(r => r != null)
            .SelectMany(r => r.Permissions)
            .Select(p => (PermissionEnum)p.Id)
            .ToHashSet();
    }
}

    