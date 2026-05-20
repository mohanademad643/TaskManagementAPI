using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Domain.Interfaces.Repositories;

public interface IProjectRepository : IGenericRepository<Project>
{
    Task<IReadOnlyList<Project>> GetProjectsByOwnerAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectWithTasksAsync(int projectId, CancellationToken cancellationToken = default);
}
