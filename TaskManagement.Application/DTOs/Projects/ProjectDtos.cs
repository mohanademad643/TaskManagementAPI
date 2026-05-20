namespace TaskManagement.Application.DTOs.Projects;

public record ProjectDto(
    int Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    int TaskCount);

public record CreateProjectDto(string Name, string? Description);

public record UpdateProjectDto(string Name, string? Description);
