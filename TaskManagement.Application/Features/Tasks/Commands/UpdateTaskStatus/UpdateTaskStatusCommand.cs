using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus
{
    public record UpdateTaskStatusCommand(int TaskId, TaskStatus Status) : IRequest<ApiResponse<TaskDto>>;
}
