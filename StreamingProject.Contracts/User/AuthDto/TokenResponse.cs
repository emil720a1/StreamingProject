namespace StreamingProject.Contracts.User.AuthDto;

public sealed record TokenResponse(string AccessToken, string RefreshToken);
