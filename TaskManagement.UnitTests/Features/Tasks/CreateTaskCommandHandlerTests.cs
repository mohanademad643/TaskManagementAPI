using Moq;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;
using TaskManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TaskManagement.UnitTests.Features.Tasks;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();

    public CreateTaskCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Tasks).Returns(_taskRepoMock.Object);
        _currentUserMock.Setup(u => u.UserId).Returns(1);
        _currentUserMock.Setup(u => u.IsInRole("Admin")).Returns(false);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        _projectRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = new CreateTaskCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
        var command = new CreateTaskCommand("Task", null, TaskPriority.High, null, 1);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NotProjectOwner_ThrowsForbiddenException()
    {
        var project = new Project { Id = 1, OwnerId = 99 }; 

        _projectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var handler = new CreateTaskCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
        var command = new CreateTaskCommand("Task", null, TaskPriority.Low, null, 1);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesTaskAndReturnsDto()
    {
        var project = new Project { Id = 1, OwnerId = 1, Name = "Test Project" };

        _projectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<ProjectTask>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateTaskCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
        var command = new CreateTaskCommand("TaskTest", "Detailed description", TaskPriority.Critical, null, 1);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal("TaskTest", result.Data!.Title);
        Assert.Equal("Todo", result.Data.Status);
    }
}
