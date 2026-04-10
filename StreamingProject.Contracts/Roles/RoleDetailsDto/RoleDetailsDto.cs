namespace StreamingProject.Contracts.Roles.RoleDetailsDto;

public record RoleDetailsDto(Guid Id, string Name, List<string> Permissions);