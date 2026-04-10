using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.User.UserRepository;
using StreamingProject.Domain.Enums;
using StreamingProject.Domain.Stream;
using StreamingProject.Domain.User;

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
        await _dbContext.Users.AddAsync(user);
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
        var deletedCount = await _dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();
        
        return deletedCount > 0;
    }

    public async Task<UserEntity?> GetUserById(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<StreamEntity>> GetStreamsByUserId(Guid userId)
    {
        return await _dbContext.Streams
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> UserExists(string username, string email)
    {
        return await _dbContext.Users
            .AnyAsync(u => u.UserName == username || u.Email == email);
    }
    

    public async Task<UserEntity?> GetByEmail(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == email);
    }

}