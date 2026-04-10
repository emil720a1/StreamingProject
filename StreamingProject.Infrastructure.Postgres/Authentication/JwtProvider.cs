using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StreamingProject.Application.Interfaces.Auth;
using StreamingProject.Contracts.User.AuthDto;
using StreamingProject.Domain.User;

namespace StreamingProject.Repository.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;
    private readonly StreamingDbContext _dbContext;

    public JwtProvider(IOptions<JwtOptions> options, StreamingDbContext dbContext)
    {
        _options = options.Value;
        _dbContext = dbContext;
    }

    public async Task<TokenResponse> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var accessToken = CreateAccessToken(user);
        var refreshTokenString = GenerateSecureToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Typically refresh tokens live longer
            IsRevoked = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new TokenResponse(accessToken, refreshTokenString);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var storedToken = await _dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenValue, cancellationToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            throw new Exception("Invalid or expired refresh token.");
        }

        storedToken.IsRevoked = true;
        _dbContext.RefreshTokens.Update(storedToken);

        return await GenerateTokenAsync(storedToken.User, cancellationToken);
    }

    private string CreateAccessToken(UserEntity user)
    {
        Claim[] claims =
        [
            new Claim(CustomClaims.UserId, user.Id.ToString()),
            new Claim(CustomClaims.UserName, user.UserName ?? string.Empty),
            new Claim(CustomClaims.Email, user.Email ?? string.Empty)
        ];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateSecureToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}