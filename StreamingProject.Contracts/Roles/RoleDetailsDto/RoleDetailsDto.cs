namespace StreamingProject.Contracts.Roles;

public record RoleDetailsDto(int Id, string Name, List<string> Permissions);