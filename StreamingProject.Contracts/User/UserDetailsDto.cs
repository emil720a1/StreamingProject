using StreamingProject.Domain.User;

namespace StreamingProject.Contracts.User;

public record UserDetailsDto(Guid Id, string Username, string FirstName, string LastName, UserRole Role, UserStatus Status);