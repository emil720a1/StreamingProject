using Microsoft.EntityFrameworkCore;
using StreamingProject.Application;
using StreamingProject.Domain.User;

namespace StreamingProject.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly StreamingDbContext _dbContext;

    public UserRepository(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserEntity?> AddUserAsync(UserEntity user)
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
}