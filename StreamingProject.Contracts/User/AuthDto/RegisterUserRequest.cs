using System.ComponentModel.DataAnnotations;

namespace StreamingProject.Application.User.AuthDto;

public record RegisterUserRequest(
    [Required] string Username,
    [Required] string Password,
    [Required] string Email
);