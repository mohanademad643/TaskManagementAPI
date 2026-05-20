using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject
{
    public record GetTasksByProjectQuery(int ProjectId) : IRequest<ApiResponse<List<TaskDto>>>;

}
