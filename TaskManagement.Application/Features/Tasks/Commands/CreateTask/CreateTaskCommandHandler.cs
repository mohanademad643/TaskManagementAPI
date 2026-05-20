using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask;



public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ApiResponse<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<TaskDto>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
       
        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        var task = new ProjectTask
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate,
            ProjectId = request.ProjectId,
            Status = TaskStatus.Todo
        };

        await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TaskDto>.Success(MapToDto(task), "Task created successfully.");
    }

    private static TaskDto MapToDto(ProjectTask task) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Status.ToString(),
        task.Priority.ToString(),
        task.DueDate,
        task.ProjectId,
        task.CreatedAt);
}
