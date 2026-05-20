
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

namespace TaskManagement.Application.Features.Auth.Commands.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResultDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly AdminSeedSettings _adminSeed;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        IOptions<AdminSeedSettings> adminSeed)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
        _adminSeed = adminSeed.Value;

    
        if (string.IsNullOrWhiteSpace(_adminSeed.Email))
            throw new InvalidOperationException(
                "AdminSeed:Email is not configured.");
    }

    public async Task<ApiResponse<AuthResultDto>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Email.Equals(_adminSeed.Email, StringComparison.OrdinalIgnoreCase))
            await EnsureAdminCreatedAsync();

        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException(nameof(ApplicationUser), request.Email);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new ForbiddenException();

        var token = await _jwtService.GenerateTokenAsync(user);

        return ApiResponse<AuthResultDto>.Success(new AuthResultDto(
            user.Id,
            user.Email!,
            $"{user.FirstName} {user.LastName}",
            token,
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)));
    }

    private async Task EnsureAdminCreatedAsync()
    {
        if (await _userManager.FindByEmailAsync(_adminSeed.Email) is not null)
            return;

        await SeedRolesAsync();

        var admin = new ApplicationUser
        {
            FirstName = _adminSeed.FirstName,
            LastName = _adminSeed.LastName,
            Email = _adminSeed.Email,
            UserName = _adminSeed.Email,
        };

        var result = await _userManager.CreateAsync(admin, _adminSeed.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            throw new ValidationException(errors);
        }

        await _userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in Enum.GetValues<UserRole>())
        {
            var roleName = role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }
}

