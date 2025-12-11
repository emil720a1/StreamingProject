using StreamingProject.Domain.Enums;

namespace StreamingProject.Contracts.Permissions;

public record RemovePermissionDto(int UserId, PermissionEnum Permission);