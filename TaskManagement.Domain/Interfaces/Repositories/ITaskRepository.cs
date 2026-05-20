using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Domain.Interfaces.Repositories;

public interface ITaskRepository : IGenericRepository<ProjectTask>
{
    Task<IReadOnlyList<ProjectTask>> GetTasksByProjectAsync(int projectId, CancellationToken cancellationToken = default);
}
