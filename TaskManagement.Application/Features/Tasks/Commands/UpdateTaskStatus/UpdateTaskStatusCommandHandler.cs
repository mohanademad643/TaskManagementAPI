using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;



public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, ApiResponse<TaskDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskStatusCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<TaskDto>> Handle(
        UpdateTaskStatusCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), request.TaskId);

        var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), task.ProjectId);

        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        task.Status = request.Status;
        _unitOfWork.Tasks.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<TaskDto>.Success(new TaskDto(
            task.Id, task.Title, task.Description,
            task.Status.ToString(), task.Priority.ToString(),
            task.DueDate, task.ProjectId, task.CreatedAt));
    }
}

