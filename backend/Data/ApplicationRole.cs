
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Data;
//inherits Roles

public class ApplicationRole : IdentityRole
{
    public string? NameArabic { get; set; }
}