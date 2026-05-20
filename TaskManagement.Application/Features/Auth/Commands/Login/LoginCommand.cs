

using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<ApiResponse<AuthResultDto>>;

}
