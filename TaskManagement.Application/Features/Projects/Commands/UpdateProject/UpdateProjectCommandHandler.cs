using MediatR;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject;


public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ApiResponse<ProjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateProjectCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<ProjectDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.Id);

      
        if (project.OwnerId != _currentUser.UserId && !_currentUser.IsInRole("Admin"))
            throw new ForbiddenException();

        project.Name = request.Name;
        project.Description = request.Description;

        _unitOfWork.Projects.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<ProjectDto>.Success(
            new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt, project.Tasks.Count));
    }
}

