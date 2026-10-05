
 using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;//ues to DbContextOptions

namespace TodoApi.Data;

public class AppDbContext : IdentityDbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) //ASP.NET Core بيعطي AppDbContext الإعدادات الخاصة بقاعدة البيانات.
        : base(options) //مرر إعدادات قاعدة البيانات إلى الـ IdentityDbContext الأساسي.
    {
    }
}