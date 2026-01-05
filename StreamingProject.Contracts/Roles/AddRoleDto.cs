namespace StreamingProject.Contracts.Roles;

public record AddRoleDto(string Name, int Id, List<string> Permissions);