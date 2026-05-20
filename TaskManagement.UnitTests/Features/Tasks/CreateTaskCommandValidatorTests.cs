using FluentValidation.TestHelper;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using Xunit;

namespace TaskManagement.UnitTests.Features.Tasks;

public class CreateTaskCommandValidatorTests
{
    private readonly CreateTaskCommandValidator _validator = new();

    [Fact]
    public async Task Validate_EmptyTitle_ShouldFail()
    {
        var command = new CreateTaskCommand("", null, TaskPriority.Low, null, 1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public async Task Validate_InvalidProjectId_ShouldFail()
    {
        var command = new CreateTaskCommand("Valid Title", null, TaskPriority.Low, null, 0);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.ProjectId);
    }

    [Fact]
    public async Task Validate_DueDateInPast_ShouldFail()
    {
        var command = new CreateTaskCommand("Valid Title", null, TaskPriority.Low, DateTime.UtcNow.AddDays(-1), 1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateTaskCommand("Fix the login bug", "Details", TaskPriority.High, DateTime.UtcNow.AddDays(3), 1);
        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
