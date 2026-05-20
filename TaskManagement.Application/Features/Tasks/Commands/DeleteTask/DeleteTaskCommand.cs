using MediatR;
using TaskManagement.Application.Common.Wrappers;

namespace TaskManagement.Application.Features.Tasks.Commands.DeleteTask
{
    public record DeleteTaskCommand(int TaskId) : IRequest<ApiResponse>;

}
