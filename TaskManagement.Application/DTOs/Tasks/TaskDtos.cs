
namespace TaskManagement.Application.DTOs.Tasks;

public record TaskDto(
    int Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    int ProjectId,
    DateTime CreatedAt);

public record CreateTaskDto(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    int ProjectId);

public record UpdateTaskStatusDto(TaskStatus Status);
