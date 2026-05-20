using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.UpdateProject
{
    public record UpdateProjectCommand(int Id, string Name, string? Description) : IRequest<ApiResponse<ProjectDto>>;

}
