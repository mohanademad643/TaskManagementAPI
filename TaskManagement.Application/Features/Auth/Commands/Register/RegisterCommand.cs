using MediatR;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Auth;

namespace TaskManagement.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand(
     string FirstName,
     string LastName,
     string Email,
     string Password) : IRequest<ApiResponse<AuthResultDto>>;
}
