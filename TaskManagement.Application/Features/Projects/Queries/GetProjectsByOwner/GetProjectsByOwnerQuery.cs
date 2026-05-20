using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectsByOwner
{
    public record GetProjectsByOwnerQuery : IRequest<ApiResponse<List<ProjectDto>>>;
 
}
