
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.Features.Auth.Commands.Login;
using TaskManagement.Application.Features.Auth.Commands.Register;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Register(
        [FromBody] RegisterDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new RegisterCommand(dto.FirstName, dto.LastName, dto.Email, dto.Password),
            cancellationToken);

        return  Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Login(
        [FromBody] LoginDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new LoginCommand(dto.Email, dto.Password),
            cancellationToken);

        return Ok(result);
    }
}