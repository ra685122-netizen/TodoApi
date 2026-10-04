
using System.Collections.Concurrent;//لتخزين الـ Refresh Tokens بطريقة مناسبة للتعامل مع أكثر من request.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;//إنشاء Refresh Token عشوائي وقوي.
using System.Text;//لتحويل الـ secret key إلى bytes
using Microsoft.IdentityModel.Tokens;
using TodoApi.Models;

namespace TodoApi.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private static readonly List<(string Username, string Password)> Users =
    [
        ("admin", "1234"),
        ("rima", "1234")
    ];
//مكان مؤقت نخزن فيه الـ Refresh Tokens
    private static readonly ConcurrentDictionary<
        string,
        (string Username, DateTime ExpiresAt)
    > RefreshTokens = new();

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResponse? Login(LoginRequest request)
    {
        var user = Users.FirstOrDefault(u =>
            u.Username == request.Username &&
            u.Password == request.Password);

        if (user == default)
        {
            return null;
        }

        return CreateTokenPair(user.Username);
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

        return CreateTokenPair(storedToken.Username);//نشئ التوكنات الجديدة
    }

    private TokenResponse CreateTokenPair(string username)
    {
        var claims = new[]          //داخل الـ JWT معلومة عن المستخدم
        {
            new Claim(ClaimTypes.Name, username)
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
            (username, DateTime.UtcNow.AddDays(30));

        return new TokenResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }
}