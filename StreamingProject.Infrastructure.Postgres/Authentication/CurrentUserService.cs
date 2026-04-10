using Microsoft.AspNetCore.Http;
using StreamingProject.Application.Interfaces.Auth;

namespace StreamingProject.Repository.Authentication;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid Id => Guid.TryParse(
        _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value, 
        out var id) ? id : Guid.Empty;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
