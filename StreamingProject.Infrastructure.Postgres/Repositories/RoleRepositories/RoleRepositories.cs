using Microsoft.EntityFrameworkCore;
using StreamingProject.Application.Service.Role.RoleRepository;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Repositories.RoleRepositories;

public class RoleRepositories : IRoleRepository
{
    private readonly StreamingDbContext _dbContext;

    public RoleRepositories(StreamingDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<RoleEntity?> GetRoleByNameAsync(string username)
    {
       return await _dbContext.Roles
           .AsNoTracking()
           .FirstOrDefaultAsync(a => a.Name == username);
    }

    public async Task<RoleEntity?> GetRoleByIdAsync(int id)
    {
        return await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
}