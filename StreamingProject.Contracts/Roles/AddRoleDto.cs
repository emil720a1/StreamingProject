namespace StreamingProject.Contracts.Roles;

public record AddRoleDto(string Name, Guid Id, List<string> Permissions);