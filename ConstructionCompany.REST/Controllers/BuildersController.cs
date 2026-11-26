using Microsoft.AspNetCore.Authorization;
using ConstructionCompany.Common;
using ConstructionCompany.Infrastructure.Models;
using ConstructionCompany.REST.Models;
using Microsoft.AspNetCore.Mvc;
namespace ConstructionCompany.REST.Controllers;
[ApiController, Route("api/builders")]
public class BuildersController(ICrudServiceAsync<BuilderModel> service) : ControllerBase
{
    private static BuilderDto Map(BuilderModel x) => new(x.Id,x.Name,x.HourlyRate,x.Specialty,x.ExperienceYears,x.SafetyCertificate);
    private static BuilderModel Entity(BuilderInput x,Guid id) => new(){Id=id,Name=x.Name,HourlyRate=x.HourlyRate,Specialty=x.Specialty,ExperienceYears=x.ExperienceYears,SafetyCertificate=x.SafetyCertificate};
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
    public async Task<IActionResult> Create(BuilderInput input) {
        var x=Entity(input,Guid.NewGuid()); if(!await service.CreateAsync(x)) return Conflict();
        return CreatedAtAction(nameof(Get),new {id=x.Id},Map(x));
    }
    [Authorize(Roles="Manager,Admin"), HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id,BuilderInput input) => await service.UpdateAsync(Entity(input,id)) ? NoContent() : NotFound();
    [Authorize(Roles="Admin"), HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) => await service.RemoveAsync(new BuilderModel{Id=id}) ? NoContent() : NotFound();
}
