using MediatR;

using TaskManagement.Application.Common.Wrappers;

namespace TaskManagement.Application.Features.Projects.Commands.DeleteProject
{
    public record DeleteProjectCommand(int Id) : IRequest<ApiResponse>;

}
