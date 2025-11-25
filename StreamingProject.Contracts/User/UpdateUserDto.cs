using StreamingProject.Domain.User;

namespace StreamingProject.Contracts.User;

public record UpdateUserDto(Guid Id, string Username, string? FirstName, string? LastName, string? Password, string Email);