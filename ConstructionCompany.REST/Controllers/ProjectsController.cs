using Microsoft.AspNetCore.Authorization;
using ConstructionCompany.Common;
using ConstructionCompany.Infrastructure.Models;
using ConstructionCompany.REST.Models;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionCompany.REST.Controllers;
[ApiController, Route("api/projects")]
public class ProjectsController(ICrudServiceAsync<ConstructionProjectModel> service) : ControllerBase
{
    private static ProjectDto Map(ConstructionProjectModel x) => new(x.Id,x.Title,x.Budget,x.Address,x.IsCompleted);
    private static ConstructionProjectModel Entity(ProjectInput x, Guid id) => new(){Id=id,Title=x.Title,Budget=x.Budget,Address=x.Address,IsCompleted=x.IsCompleted};
    [HttpGet]
    public async Task<IActionResult> All(int page=1,int amount=20) {
        if(page<1 || amount<1 || amount>100) return BadRequest();
        return Ok((await service.ReadAllAsync(page,amount)).Select(Map));
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) {
        try { return Ok(Map(await service.ReadAsync(id))); } catch(KeyNotFoundException) { return NotFound(); }
    }
    [Authorize(Roles="Manager,Admin"), HttpPost]
    public async Task<IActionResult> Create(ProjectInput input) {
        var x=Entity(input,Guid.NewGuid()); if(!await service.CreateAsync(x)) return Conflict();
        return CreatedAtAction(nameof(Get),new {id=x.Id},Map(x));
    }
    [Authorize(Roles="Manager,Admin"), HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id,ProjectInput input) => await service.UpdateAsync(Entity(input,id)) ? NoContent() : NotFound();
    [Authorize(Roles="Admin"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) => await service.RemoveAsync(new ConstructionProjectModel{Id=id}) ? NoContent() : NotFound();
}
