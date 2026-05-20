using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectById
{
    public record GetProjectByIdQuery(int Id) : IRequest<ApiResponse<ProjectDto>>;

}
