using System.ComponentModel.DataAnnotations;

namespace StreamingProject.Contracts.User.AuthDto;

public record LoginUserRequest(
   [Required] string Email,
    [Required] string Password
    );