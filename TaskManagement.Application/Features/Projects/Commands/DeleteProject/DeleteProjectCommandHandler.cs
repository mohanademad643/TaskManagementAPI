using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject;


public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.Id);

        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        _unitOfWork.Projects.Delete(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResult("Project deleted successfully.");
    }
}
