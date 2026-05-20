using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask;


public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteTaskCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(ProjectTask), request.TaskId);

        var project = await _unitOfWork.Projects.GetByIdAsync(task.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), task.ProjectId);

        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        _unitOfWork.Tasks.Delete(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResult("Task deleted successfully.");
    }
}
