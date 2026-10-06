
using Microsoft.AspNetCore.Identity;

namespace TodoApi.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)      //عطينا إمكانية الحصول على الخدمات المسجلة في Dependency Injection.
    {
        var roleManager =
            services.GetRequiredService<RoleManager<ApplicationRole>>();

        var userManager =
            services.GetRequiredService<UserManager<ApplicationUser>>(); //المسؤول عن إدارة المستخدمين
        //const  القيمة ثابتة داخل هذا الكود ولا نغيرها أثناء التشغيل
        const string roleName = "Admin";
        const string username = "admin";
        const string password = "1234";
        

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(
    new ApplicationRole
    {
        Name = roleName,
        NameArabic = "مدير النظام"
    }
);
        }

        var user = await userManager.FindByNameAsync(username);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = username,
                Name = "Admin",
                NameArabic = "مدير النظام",
                LockoutEnabled = false
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
       
        const string normalUsername = "user";
        const string normalPassword = "1234";

        var normalUser =
            await userManager.FindByNameAsync(normalUsername);

        if (normalUser is null)
        {
            normalUser = new ApplicationUser
            {
                UserName = normalUsername,
                Name = "User",
                NameArabic = "مستخدم",
                LockoutEnabled = true //يُقفل بعد 3 محاولات فاشلة
            };

            var result = await userManager.CreateAsync(
                normalUser,
                normalPassword
            );

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(
                        "; ",
                        result.Errors.Select(e => e.Description)
                    )
                );
            }
        }
    }
}