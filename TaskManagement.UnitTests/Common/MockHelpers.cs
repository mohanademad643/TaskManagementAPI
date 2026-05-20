using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TaskManagement.Domain.Entities;

namespace TaskManagement.UnitTests.Common;

public static class MockHelpers
{
    public static Mock<UserManager<ApplicationUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var idOptions = new IdentityOptions();
        options.Setup(o => o.Value).Returns(idOptions);

        var manager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            options.Object,
            new PasswordHasher<ApplicationUser>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null!,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        manager.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
        manager.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

        return manager;
    }

    public static Mock<RoleManager<IdentityRole<int>>> MockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole<int>>>();
        return new Mock<RoleManager<IdentityRole<int>>>(
            store.Object,
            Array.Empty<IRoleValidator<IdentityRole<int>>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<IdentityRole<int>>>>().Object);
    }

   
    public static ApplicationUser BuildUser(int id = 1, string email = "test@test.com")
        => new()
        {
            Id = id,
            Email = email,
            UserName = email,
            FirstName = "Test",
            LastName = "User"
        };
}
