using FluentValidation.TestHelper;
using TaskManagement.Application.Features.Auth.Commands.Register;
using Xunit;

namespace TaskManagement.UnitTests.Features.Auth;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Theory]
    [InlineData("", "User", "email@test.com", "Password123!")]
    [InlineData("Test", "", "email@test.com", "Password123!")]
    [InlineData("Test", "User", "not-an-email", "Password123!")]
    [InlineData("Test", "User", "email@test.com", "short")]
    public async Task Validate_InvalidInputs_ShouldHaveErrors(
        string firstName, string lastName, string email, string password)
    {
        var command = new RegisterCommand(firstName, lastName, email, password);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public async Task Validate_ValidInput_ShouldPassValidation()
    {
        var command = new RegisterCommand("John", "Doe", "john@test.com", "Password123!");
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
