using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApi.Services;
using Scalar.AspNetCore;
using TodoApi.Middleware;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor(); //تحتاج خدمة إلى معرفة المستخدم الذي أرسل الطلب حتى تتعامل مع مهامه.   //الـ JWT مسؤول عن التحقق من هوية المستخدم، بينما HttpContextAccessor يتيح الوصول إلى هذه الهوية من خلال HttpContext.User

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
builder.Services 
        .AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequiredLength = 4;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Lockout.MaxFailedAccessAttempts = 3;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
            options.Lockout.AllowedForNewUsers = true;
        })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>()          //خزّن بيانات Identity في قاعدة البيانات عن طريق AppDbContext.
.AddSignInManager(); //SignInManager مسؤول عن عملية تسجيل الدخول في Identity.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters //هذه هي القواعد التي يجب أن يمر بها الـ JWT حتى نعتبره صحيحًا.
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });
builder.Services.AddOpenApi();
var app = builder.Build();
using (var scope = app.Services.CreateScope())           
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();