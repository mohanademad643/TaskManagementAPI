
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Domain.Entities;
using TaskManagement.Infastructure.Settings;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Settings;

namespace TaskManagement.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly AdminSeedSettings _adminSeed;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        IOptions<AdminSeedSettings> adminSeed)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
        _adminSeed = adminSeed.Value;
    }

    public async Task<ApiResponse<AuthResultDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Email.Equals(_adminSeed.Email, StringComparison.OrdinalIgnoreCase))
            throw new ConflictException("This email address is reserved.");

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(user, UserRole.User.ToString());

        var token = await _jwtService.GenerateTokenAsync(user);

        return ApiResponse<AuthResultDto>.Success(new AuthResultDto(
            user.Id,
            user.Email!,
            $"{user.FirstName} {user.LastName}",
            token,
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)));
    }
}

