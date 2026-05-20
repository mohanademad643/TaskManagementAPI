using Moq;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;
using TaskManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TaskManagement.UnitTests.Features.Projects;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();

    public CreateProjectCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepoMock.Object);
        _currentUserMock.Setup(u => u.UserId).Returns(1);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProjectAndReturnsDto()
    {
        // Arrange
        _projectRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateProjectCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);
        var command = new CreateProjectCommand("My Project", "Description here");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal("My Project", result.Data.Name);

        _projectRepoMock.Verify(r => r.AddAsync(It.Is<Project>(p =>
            p.Name == "My Project" && p.OwnerId == 1), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
