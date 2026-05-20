using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects
{
    public record GetAllProjectsQuery : IRequest<ApiResponse<List<ProjectDto>>>;
}
