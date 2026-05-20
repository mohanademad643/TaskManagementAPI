
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Tasks.Commands.CreateTask;
using TaskManagement.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using TaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator) => _mediator = mediator;

    // Any authenticated user can view tasks under a project
    [HttpGet("Get-Task-GetBy-ProjectId/{projectId:int}")]
    public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetByProject(
        int projectId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId), cancellationToken);
        return Ok(result);
    }

    // Admin only — creates a task inside a project
    [HttpPost("Create-Task")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ApiResponse<TaskDto>>> Create(
        [FromBody] CreateTaskDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateTaskCommand(dto.Title, dto.Description, dto.Priority, dto.DueDate, dto.ProjectId),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    // Any authenticated user can update the status of a task
    [HttpPatch("Update/{taskId:int}/status")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateStatus(
        int taskId,
        [FromBody] UpdateTaskStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTaskStatusCommand(taskId, dto.Status), cancellationToken);
        return Ok(result);
    }

    // Admin only — permanently deletes a task
    [HttpDelete("Delete-Task/{taskId:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ApiResponse<object>>> Delete(
        int taskId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(taskId), cancellationToken);
        return Ok(result);
    }
}