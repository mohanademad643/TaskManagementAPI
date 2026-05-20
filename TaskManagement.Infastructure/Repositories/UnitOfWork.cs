using TaskManagement.Domain.Interfaces.Common;
using TaskManagement.Domain.Interfaces.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IProjectRepository? _projects;
    private ITaskRepository? _tasks;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    public IProjectRepository Projects
        => _projects ??= new ProjectRepository(_context);

    public ITaskRepository Tasks
        => _tasks ??= new TaskRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
        => _context.Dispose();
}
