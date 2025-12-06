using StreamingProject.Domain.Enums;

namespace StreamingProject.Contracts.Permissions;

public record RemovePermissionDto(int UserId, string Name, PermissionEnum Permission);