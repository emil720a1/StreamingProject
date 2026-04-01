using Microsoft.EntityFrameworkCore;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Repositories.UserRepositories;

public class UserSeeder : ISeeder
{
    private readonly StreamingDbContext _context;

    public UserSeeder(StreamingDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedAsync(StreamingDbContext context)
    {
        if (await context.Users.AnyAsync()) return;
        
        var adminRoleId = (int)RoleEnum.Admin;
        var adminRole = await _context.Roles.FindAsync(adminRoleId);

        if (adminRole == null)
        {
            adminRole = new RoleEntity
            {
                Id = adminRoleId,
                Name = "Admin"
            };
            await _context.Roles.AddAsync(adminRole);
            await _context.SaveChangesAsync();
        }

        var admin = UserEntity.Create(
            "admin",
            "hashed_password_here",
            "admin@stream.com",
            adminRole,
            "AdminFirstName",
            "AdminLastName");
        
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}