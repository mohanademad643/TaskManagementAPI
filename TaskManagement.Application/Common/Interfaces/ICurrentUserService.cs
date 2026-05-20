namespace TaskManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}
