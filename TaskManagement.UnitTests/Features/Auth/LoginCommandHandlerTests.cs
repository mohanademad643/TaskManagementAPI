using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Features.Auth.Commands.Login;
using TaskManagement.Infrastructure.Identity;
using TaskManagement.Infrastructure.Settings;
using TaskManagement.Infastructure.Settings;
using TaskManagement.UnitTests.Common;

namespace TaskManagement.UnitTests.Features.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<Domain.Entities.ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly IOptions<AdminSeedSettings> _adminSeedOptions;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager();
        _roleManagerMock = MockHelpers.MockRoleManager();
        _jwtServiceMock = new Mock<IJwtService>();

        _jwtOptions = Options.Create(new JwtSettings
        {
            SecretKey = "MmLZO3Udv255sMLmIPBphtjyz4oVI86H0iYoyVgWGOsxKjhHWpU8tyyOywjxhr0x",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        });

        _adminSeedOptions = Options.Create(new AdminSeedSettings
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@taskmanagement.com",
            Password = "Admin@1234!"
        });

        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _jwtServiceMock.Object,
            _jwtOptions,
            _adminSeedOptions);
    }


    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResult()
    {
        // Arrange
        var user = MockHelpers.BuildUser();
        _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Password123!")).ReturnsAsync(true);
        _jwtServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync("test-jwt-token");

        var command = new LoginCommand(user.Email!, "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal("test-jwt-token", result.Data.Token);
        Assert.Equal(user.Email, result.Data.Email);
    }


    [Fact]
    public async Task Handle_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Domain.Entities.ApplicationUser?)null);

        var command = new LoginCommand("notfound@test.com", "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }


    [Fact]
    public async Task Handle_WrongPassword_ThrowsForbiddenException()
    {
        // Arrange
        var user = MockHelpers.BuildUser();
        _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var command = new LoginCommand(user.Email!, "WrongPassword");

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }


    [Fact]
    public async Task Handle_AdminFirstLogin_SeedsAdminAndReturnsAuthResult()
    {
        var adminEmail = _adminSeedOptions.Value.Email;
        var adminPassword = _adminSeedOptions.Value.Password;
        var adminUser = MockHelpers.BuildUser(id: 99, email: adminEmail);

 
        _userManagerMock
            .SetupSequence(x => x.FindByEmailAsync(adminEmail))
            .ReturnsAsync((Domain.Entities.ApplicationUser?)null) 
            .ReturnsAsync(adminUser);                              

        _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.ApplicationUser>(), adminPassword))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<Domain.Entities.ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(adminUser, adminPassword)).ReturnsAsync(true);
        _jwtServiceMock.Setup(x => x.GenerateTokenAsync(adminUser)).ReturnsAsync("admin-jwt-token");

        var command = new LoginCommand(adminEmail, adminPassword);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("admin-jwt-token", result.Data!.Token);
        Assert.Equal(adminEmail, result.Data.Email);

        _userManagerMock.Verify(x => x.CreateAsync(
            It.Is<Domain.Entities.ApplicationUser>(u => u.Email == adminEmail),
            adminPassword), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(
            It.IsAny<Domain.Entities.ApplicationUser>(),
            nameof(UserRole.Admin)), Times.Once);
    }

    [Fact]
    public async Task Handle_AdminAlreadyExists_SkipsSeedAndReturnsAuthResult()
    {
        // Arrange 
        var adminEmail = _adminSeedOptions.Value.Email;
        var adminUser = MockHelpers.BuildUser(id: 99, email: adminEmail);

        _userManagerMock.Setup(x => x.FindByEmailAsync(adminEmail)).ReturnsAsync(adminUser);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(adminUser, _adminSeedOptions.Value.Password))
            .ReturnsAsync(true);
        _jwtServiceMock.Setup(x => x.GenerateTokenAsync(adminUser)).ReturnsAsync("admin-jwt-token");

        var command = new LoginCommand(adminEmail, _adminSeedOptions.Value.Password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
      
        _userManagerMock.Verify(x => x.CreateAsync(
            It.IsAny<Domain.Entities.ApplicationUser>(),
            It.IsAny<string>()), Times.Never);
    }
}