using ConstructionCompany.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionCompany.REST.Controllers;
[ApiController, Route("api/users"), Authorize(Roles="Admin")]
public class RolesController(UserManager<ApplicationUser> users):ControllerBase
{
    [HttpPut("{id}/role")]
    public async Task<IActionResult> SetRole(string id, [FromBody] RoleInput input) {
        if(input.Role is not ("Viewer" or "Manager")) return BadRequest("Only Viewer or Manager may be assigned here.");
        var user=await users.FindByIdAsync(id);
        if(user==null) return NotFound();
        if(await users.IsInRoleAsync(user,"Admin")) return BadRequest("Admin role is protected.");
        var existing=await users.GetRolesAsync(user);
        var added=await users.AddToRoleAsync(user,input.Role);
        if(!added.Succeeded && !existing.Contains(input.Role)) return BadRequest(added.Errors);
        var removed=await users.RemoveFromRolesAsync(user,existing.Where(r=>r!=input.Role));
        if(!removed.Succeeded)return BadRequest(removed.Errors);
        await users.UpdateSecurityStampAsync(user);
        return NoContent();
    }
    [HttpGet]
    public IActionResult All()=>Ok(users.Users.Select(u=>new {u.Id,u.Email}).ToList());
}
public record RoleInput(string Role);
