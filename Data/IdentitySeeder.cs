
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)      //عطينا إمكانية الحصول على الخدمات المسجلة في Dependency Injection.
    {
        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        var userManager =
            services.GetRequiredService<UserManager<IdentityUser>>(); //المسؤول عن إدارة المستخدمين
        //const  القيمة ثابتة داخل هذا الكود ولا نغيرها أثناء التشغيل
        const string roleName = "Admin";
        const string username = "admin";
        const string password = "1234";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(
                new IdentityRole(roleName)
            );
        }

        var user = await userManager.FindByNameAsync(username);

        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = username
            };

            var result = await userManager.CreateAsync(
                user,
                password
            );

            if (!result.Succeeded)
            {
                throw new InvalidOperationException( //وقف العملية ونرمي Exception.
                    string.Join(
                        "; ",
                        result.Errors.Select(e => e.Description) //بدل ما نعرف فقط أن العملية فشلت، نعرف ليش فشلت
                    )
                );
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}