using StreamingProject.Domain.Enums;

namespace StreamingProject.Contracts.Permissions;

public record UpdatePermissionDto(int UserId, string Name, HashSet<PermissionEnum> Permissions);