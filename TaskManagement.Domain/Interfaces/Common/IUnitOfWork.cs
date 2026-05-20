
using TaskManagement.Domain.Interfaces.Repositories;

namespace TaskManagement.Domain.Interfaces.Common
{
    public interface IUnitOfWork : IDisposable
    {
        IProjectRepository Projects { get; }
        ITaskRepository Tasks { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}
