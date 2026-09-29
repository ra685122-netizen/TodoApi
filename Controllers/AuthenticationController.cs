
using Microsoft.AspNetCore.Mvc;//حتاجه عشان نستخدم أدوات ASP.NET Core الخاصة بالـ Controllers
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;//يعني الأدوات التي تساعدنا نبني JWT ونحوّله إلى النص النهائي.
using System.Security.Claims;
using System.Text;
using TodoApi.Models;//عشان الـ Controller يقدر يستخدمLoginRequest


namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    private static readonly List<(string Username, string Password)> Users =
    [
        ("admin", "1234"),
        ("rima", "1234")
    ];

    public AuthenticationController(IConfiguration configuration)//DI
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = Users.FirstOrDefault(u =>
            u.Username == request.Username &&
            u.Password == request.Password);

        if (user == default)
        {
            return Unauthorized("Invalid username or password.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username)//عبارة عن معلومة نضعها داخل الـ JWT عن المستخدم
        };

        var key = new SymmetricSecurityKey(             //Symmetric يعني أننا نستخدم نفس المفتاح السري في عملية التوقيع والتحقق.
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(//نشاء JWT نفسه
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials//هنا نعطي الـ JWT معلومات التوقيع:
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);//تحويل JWT إلى النص النهائي
        //استخدم هذا المفتاح مع خوارزمية HmacSha256 لتوقيع الـ JWT.

        return Ok(new
        {
            token = tokenString
        });
    }
}