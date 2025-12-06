using StreamingProject.Domain.Enums;

namespace StreamingProject.Contracts.Permissions;

public record AddPermissionDto(int UserId, string Name ,PermissionEnum Permission);