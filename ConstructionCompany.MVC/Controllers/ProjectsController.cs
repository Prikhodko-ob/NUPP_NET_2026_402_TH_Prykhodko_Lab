using ConstructionCompany.Infrastructure;
using ConstructionCompany.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ConstructionCompany.MVC.Controllers;
public class ProjectsController(ConstructionCompanyContext db):Controller
{
    public async Task<IActionResult> Index()=>View(await db.Projects.AsNoTracking().ToListAsync());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, decimal budget, string address){
        if(string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(address)||budget<0) return BadRequest();
        db.Projects.Add(new ConstructionProjectModel{Title=title,Budget=budget,Address=address});
        await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id){
        var x=await db.Projects.FindAsync(id); if(x==null)return NotFound();
        db.Projects.Remove(x); await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
