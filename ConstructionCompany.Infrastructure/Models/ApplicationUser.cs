using Microsoft.AspNetCore.Identity;
namespace ConstructionCompany.Infrastructure.Models;
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = "";
}
