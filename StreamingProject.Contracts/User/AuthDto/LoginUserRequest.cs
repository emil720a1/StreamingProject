using System.ComponentModel.DataAnnotations;

namespace StreamingProject.Application.User.AuthDto;

public record LoginUserRequest(
   [Required] string Email,
    [Required] string Password
    );