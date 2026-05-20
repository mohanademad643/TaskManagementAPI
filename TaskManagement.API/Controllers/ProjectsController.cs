using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Common.Wrappers;
using TaskManagement.Application.DTOs.Projects;
using TaskManagement.Application.DTOs.Tasks;
using TaskManagement.Application.Features.Projects.Commands.CreateProject;
using TaskManagement.Application.Features.Projects.Commands.DeleteProject;
using TaskManagement.Application.Features.Projects.Commands.UpdateProject;
using TaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using TaskManagement.Application.Features.Projects.Queries.GetProjectById;
using TaskManagement.Application.Features.Projects.Queries.GetProjectsByOwner;

namespace TaskManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Get-All-Projects")]
  
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllProjectsQuery(), cancellationToken);
        return Ok(result);
    }
    [HttpGet("Get-Projects-By-Owner")]

    public async Task<ActionResult<ApiResponse<TaskDto>>> GetAllOwnerProduct(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectsByOwnerQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("Get-Project-By-Id/{id:int}")]
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("Create-Project")]
    
    public async Task<ActionResult<ApiResponse<TaskDto>>> Create([FromBody] CreateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateProjectCommand(dto.Name, dto.Description), cancellationToken);
        return  StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("Update-Project/{id:int}")]
   
    public async Task<ActionResult<ApiResponse<TaskDto>>> Update(int id, [FromBody] UpdateProjectDto dto, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProjectCommand(id, dto.Name, dto.Description), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("Delete/Project{id:int}")]
    
    public async Task<ActionResult<ApiResponse<TaskDto>>> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return Ok(result);
    }
}
