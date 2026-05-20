using MediatR;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Domain.Interfaces.Common;

namespace TaskManagement.Application.Features.Projects.Queries.GetProjectsByOwner
{
    internal class GetProjectsByOwnerQueryHandler : IRequestHandler<GetProjectsByOwnerQuery, ApiResponse<List<ProjectDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;

        public GetProjectsByOwnerQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<ApiResponse<List<ProjectDto>>> Handle(GetProjectsByOwnerQuery request, CancellationToken cancellationToken)
        {
            var projects = await _unitOfWork.Projects
                .GetProjectsByOwnerAsync(_currentUser.UserId,cancellationToken);

            var result = projects
                .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt, p.Tasks.Count))
                .ToList();

            return ApiResponse<List<ProjectDto>>.Success(result);
        }
    }
}
