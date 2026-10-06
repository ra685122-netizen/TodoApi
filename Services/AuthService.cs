
using System.Collections.Concurrent;//لتخزين الـ Refresh Tokens بطريقة مناسبة للتعامل مع أكثر من request.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;//إنشاء Refresh Token عشوائي وقوي.
using System.Text;//لتحويل الـ secret key إلى bytes
using Microsoft.AspNetCore.Identity;
using TodoApi.Data;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;


namespace TodoApi.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    //مكان مؤقت نخزن فيه الـ Refresh Tokens
    private static readonly ConcurrentDictionary<
        string,
      (string Username, string Role, DateTime ExpiresAt)
    > RefreshTokens = new();

    public AuthService(
    UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }
    public async Task<TokenResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
        {
            return null;
        }
        var result = await _signInManager.CheckPasswordSignInAsync(  //إذا كلمة المرور غلط، سجّل هذه كمحاولة فاشلة وطبّق نظام Lockout.
      user,
      request.Password,
      lockoutOnFailure: true
  );

        if (!result.Succeeded)
        {
            return null;
        }
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";
        return CreateTokenPair(user.UserName!, role);
    }


    public TokenResponse? RefreshToken(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        if (!RefreshTokens.TryGetValue(refreshToken, out var storedToken))//ابحث عن الـ Refresh Token الذي أرسله المستخدم
        {
            return null;
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)//نتأكد أنه ما انتهى
        {
            RefreshTokens.TryRemove(refreshToken, out _);
            return null;
        }

        RefreshTokens.TryRemove(refreshToken, out _);

        return CreateTokenPair(storedToken.Username, storedToken.Role);
    }

    private TokenResponse CreateTokenPair(string username, string role)
    {
        var claims = new[]          //داخل الـ JWT معلومة عن المستخدم
        {
               new Claim(ClaimTypes.Name, username),
               new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(                    //يحول النص إلى bytes.
                _configuration["Jwt:Key"]!
            )
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: credentials
        );

        var accessToken =      //تحويله إلى Token فعلي
            new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken =
            Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
            );

        RefreshTokens[refreshToken] =
         (username, role, DateTime.UtcNow.AddDays(30));

        return new TokenResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }
}