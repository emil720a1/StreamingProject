using Microsoft.EntityFrameworkCore;
using StreamingProject.Domain;
using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Repositories.UserRepositories;

public class UserSeeder : ISeeder
{
    private static readonly Guid AdminRoleId = new Guid("00000000-0000-0000-0000-000000000001");

    public async Task SeedAsync(StreamingDbContext context)
    {
        if (await context.Users.AnyAsync()) return;
        
        var adminRole = await context.Roles.FindAsync(AdminRoleId);

        if (adminRole == null)
        {
            adminRole = RoleEntity.Create(AdminRoleId, RoleEnum.Admin.ToString());
            await context.Roles.AddAsync(adminRole);
            await context.SaveChangesAsync();
        }

        var admin = UserEntity.Create(
            "admin",
            "AQAAAAIAAYagAAAAEG1m8...",
            "admin@stream.com",
            adminRole,
            "AdminFirstName",
            "AdminLastName");
        
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}