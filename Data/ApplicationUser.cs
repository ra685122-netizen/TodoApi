
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Data;
//inherits users
public class ApplicationUser : IdentityUser
{
    public string? Name { get; set; }

    public string? NameArabic { get; set; }
}