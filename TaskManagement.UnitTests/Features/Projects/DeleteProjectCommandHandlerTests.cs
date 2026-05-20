using Moq;
using TaskManagement.Application.Common.Exceptions;
using TaskManagement.Application.Common.Interfaces;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Common;
using TaskManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TaskManagement.UnitTests.Features.Projects;

public class DeleteProjectCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();

    public DeleteProjectCommandHandlerTests()
    {
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _projectRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteProjectCommand(99), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NotOwner_ThrowsForbiddenException()
    {
        // Arrange
        var project = new Project { Id = 1, OwnerId = 5, Name = "Someone Else's Project" };

        _projectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _currentUserMock.Setup(u => u.UserId).Returns(1); 
        _currentUserMock.Setup(u => u.IsInRole("Admin")).Returns(false);

        var handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(new DeleteProjectCommand(1), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_OwnerDeletes_Succeeds()
    {
        // Arrange
        var project = new Project { Id = 1, OwnerId = 1, Name = "My Project" };

        _projectRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _projectRepoMock.Setup(r => r.Delete(project));

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _currentUserMock.Setup(u => u.UserId).Returns(1);
        _currentUserMock.Setup(u => u.IsInRole("Admin")).Returns(false);

        var handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object, _currentUserMock.Object);

        // Act
        var result = await handler.Handle(new DeleteProjectCommand(1), CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        _projectRepoMock.Verify(r => r.Delete(project), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
