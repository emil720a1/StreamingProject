using StreamingProject.Domain;
using StreamingProject.Domain.User;

namespace StreamingProject.Contracts.User;

public record AddUserDto(string Username, string Password , string Email, string? FirstName, string? LastName);