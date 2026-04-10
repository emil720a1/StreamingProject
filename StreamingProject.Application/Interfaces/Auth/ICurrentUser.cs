namespace StreamingProject.Application.Interfaces.Auth;

public interface ICurrentUser
{
    Guid Id { get; }
    bool IsAuthenticated { get; }
}
