using JSONWebTokenAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly DynamicAccess _roleService;
    private readonly UserRoleService _userRoleService;

  
    public AdminController(DynamicAccess roleService, UserRoleService userRoleService)
    {
        _roleService = roleService;
        _userRoleService = userRoleService;
    }

    
    [HttpPost("createrole/{roleName}")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
       
        var created = await _roleService.CreateRoleWithAccessAsync(roleName);
        if (created)
        {
            return Ok($"Role '{roleName}' created."); // Success response
        }
        else
        {
            return BadRequest("Role creation failed or already exists."); // Failure response
        }
    }

    // HTTP POST endpoint to assign an existing role to a user
    [HttpPost("assignrole")]
    public async Task<IActionResult> AssignRoleToUser(string userId, string roleName)
    {
        // Calls service to assign role to specified user
        var assigned = await _userRoleService.AssignRoleToUserAsync(userId, roleName);
        if (assigned)
        {
            return Ok($"Role '{roleName}' assigned to user."); // Success response
        }
        else
        {
            return BadRequest("Role assignment failed."); // Failure response
        }
    }
}
