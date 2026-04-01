namespace StreamingProject.Contracts.Roles.RoleDetailsDto;

public record RoleDetailsDto(int Id, string Name, List<string> Permissions);