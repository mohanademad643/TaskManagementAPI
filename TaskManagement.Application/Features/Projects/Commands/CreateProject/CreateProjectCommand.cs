using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Commands.CreateProject
{
    public record CreateProjectCommand(string Name, string? Description) : IRequest<ApiResponse<ProjectDto>>;
}
