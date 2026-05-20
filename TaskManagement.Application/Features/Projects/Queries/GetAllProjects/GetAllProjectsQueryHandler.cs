
using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, ApiResponse<List<ProjectDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    public GetAllProjectsQueryHandler(IUnitOfWork unitOfWork )
    {
        _unitOfWork = unitOfWork;
        
    }

    public async Task<ApiResponse<List<ProjectDto>>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _unitOfWork.Projects
            .GetAllAsync( cancellationToken);

        var result = projects
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt, p.Tasks.Count))
            .ToList();

        return ApiResponse<List<ProjectDto>>.Success(result);
    }
}