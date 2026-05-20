using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Identity;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
