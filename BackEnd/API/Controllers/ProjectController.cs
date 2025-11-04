using CORE.Abstractions;
using CORE.Entities;
using CORE.Models;
using CORE.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class ProjectController : ControllerBase
{

    private readonly IProjectService _projectService;
    private readonly IRepository<ProjectEntity> _repository;

    public ProjectController(IProjectService projectService, IRepository<ProjectEntity> repository)
    {
        _projectService = projectService;
        _repository = repository;   
    }

    [Authorize]
    [HttpGet]
    [Route("list-all")]
    public async Task<IActionResult> GetProjects([FromQuery(Name = "name")] string name)
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name);
        var username = usernameClaim?.Value ?? string.Empty;

        var rs = await _projectService.GetAllProjectByUsernameOrName(username, name);
        return Ok(rs);
    }

    [Authorize]
    [HttpGet]
    [Route("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery(Name = "id")] string id)
    {
        var rs = await _projectService.GetProjectWithTasks(Guid.Parse(id));
        return Ok(rs);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    [Route("create-or-update")]
    public async Task<IActionResult> CreateOrUpdateProject([FromBody] ProjectRequestModel model)
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name);
        var username = usernameClaim?.Value ?? string.Empty;
        model.Username = username;
        var rs = await _projectService.CreateOrUpdateProject(model);
        if (rs.ResultCode == 0) return BadRequest(rs.Message);
        return StatusCode(201, rs.Message);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid id)
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name);
        var username = usernameClaim?.Value ?? string.Empty;
        var rs = await _projectService.DeleteProject(id, username);
        return Ok(rs);
    }
}
