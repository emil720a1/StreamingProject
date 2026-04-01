using Microsoft.EntityFrameworkCore;
 using StreamingProject.Application.Service.Role.RoleRepository;
 using StreamingProject.Domain.User.UserRole;
 
 namespace StreamingProject.Repository.Repositories.RoleRepositories;
 
 public class RoleRepository : IRoleRepository
 {
     private readonly StreamingDbContext _dbContext;
 
     public RoleRepository(StreamingDbContext dbContext)
     {
         _dbContext = dbContext;
     }

     public async Task<RoleEntity> AddRoleAsync(RoleEntity role)
     {
         await _dbContext.Roles.AddAsync(role);
         await _dbContext.SaveChangesAsync();

         return role;
     }

     public async Task<RoleEntity?> GetRoleByNameAsync(string name)
     {
        return await _dbContext.Roles
            .AsNoTracking()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(a => a.Name == name);
     }
 
     public async Task<RoleEntity?> GetRoleByIdAsync(int id)
     {
         return await _dbContext.Roles
             .Include(r => r.Permissions)
             .FirstOrDefaultAsync(a => a.Id == id);
     }

     public async Task<RoleEntity> UpdateRoleAsync(RoleEntity role)
     {
         _dbContext.Roles.Update(role);
         await _dbContext.SaveChangesAsync();

         return role;
     }

     public async Task<bool> DeleteRoleAsync(int id)
     {
         var deletedCount = await _dbContext.Roles
             .Where(r => r.Id == id)
             .ExecuteDeleteAsync();

         return deletedCount > 0;
     }
 }