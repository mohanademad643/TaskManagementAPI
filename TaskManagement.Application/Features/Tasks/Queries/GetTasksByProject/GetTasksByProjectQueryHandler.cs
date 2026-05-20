using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, ApiResponse<List<TaskDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public GetTasksByProjectQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<List<TaskDto>>> Handle(
        GetTasksByProjectQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(request.ProjectId, cancellationToken);

        var result = tasks
            .Select(t => new TaskDto(
                t.Id, t.Title, t.Description,
                t.Status.ToString(), t.Priority.ToString(),
                t.DueDate, t.ProjectId, t.CreatedAt))
            .ToList();

        return ApiResponse<List<TaskDto>>.Success(result);
    }
}