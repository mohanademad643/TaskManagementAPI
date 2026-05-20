using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;

namespace TaskManagement.Application.Features.Tasks.Commands.CreateTask
{
    public record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int ProjectId) : IRequest<ApiResponse<TaskDto>>;
}
