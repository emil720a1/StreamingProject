using StreamingProject.Domain.User;
using StreamingProject.Domain.User.UserRole;

namespace StreamingProject.Contracts.User;

public record UserDetailsDto(
    Guid Id,
    string Username, 
    string FirstName, 
    string LastName, 
    ICollection<string> Roles, 
    Enum Status);