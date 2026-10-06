
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;//إنشاء Refresh Token عشوائي وقوي.
using System.Text;//لتحويل الـ secret key إلى bytes

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Services;
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AppDbContext _dbContext; //الباب الذي نتعامل من خلاله مع قاعدة البيانات

    public AuthService(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      AppDbContext dbContext,
      IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _dbContext = dbContext;
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

        return await CreateTokenPairAsync(
            user.UserName!,
            user.Id,
            role
        );
    }

    public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (storedToken is null)
        {
            return null;
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            _dbContext.RefreshTokens.Remove(storedToken);
            await _dbContext.SaveChangesAsync();

            return null;
        }

        var user = await _userManager.FindByIdAsync(storedToken.UserId); //البحث عن المستخدم

        if (user is null)
        {
            return null;
        }

        _dbContext.RefreshTokens.Remove(storedToken); //حذف الـ Refresh Token بعد استخدامه
        await _dbContext.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return await CreateTokenPairAsync(
            user.UserName!,
             user.Id,
              role);
    }
    private async Task<TokenResponse> CreateTokenPairAsync(
        string username,
        string userId,
        string role)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
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

        var accessToken =
            new JwtSecurityTokenHandler().WriteToken(token);
        //نولد قيمة عشوائية قوية
        var refreshTokenValue =
            Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
            );

        var refreshToken = new RefreshToken
        {
            //السجل الذي سيدخل جدول RefreshTokens
            Token = refreshTokenValue,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync();

        return new TokenResponse
        {
            Token = accessToken,
            RefreshToken = refreshTokenValue
        };
    }
}