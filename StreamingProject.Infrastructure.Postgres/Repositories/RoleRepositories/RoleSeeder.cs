using Microsoft.EntityFrameworkCore;
using StreamingProject.Domain;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Repository.Repositories.RoleRepositories;

public class RoleSeeder : ISeeder
{
    private static readonly Guid AdminRoleId =
        new("00000000-0000-0000-0000-000000000001");

    private static readonly Guid UserRoleId =
        new("00000000-0000-0000-0000-000000000002");

    public async Task SeedAsync(StreamingDbContext context)
    {
        var roles = new[]
        {
            (AdminRoleId, RoleEnum.Admin.ToString()),
            (UserRoleId, RoleEnum.User.ToString()),
        };

        foreach (var (id, name) in roles)
        {
            var exists = await context.Roles
                .SingleOrDefaultAsync(role => role.Name == name);

            if (exists is null)
            {
                await context.Roles.AddAsync(
                    RoleEntity.Create(id, name));
            }
            else if (exists.NormalizedName != name.ToUpperInvariant())
            {
                // Repair roles created by the old seed/migration.
                exists.NormalizedName = name.ToUpperInvariant();
            }
        }

        await context.SaveChangesAsync();
    }
}
