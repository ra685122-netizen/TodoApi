using TodoApi.Models;

namespace TodoApi.Services;

public interface IAuthService
{
    Task<TokenResponse?> LoginAsync(LoginRequest request);
    TokenResponse? RefreshToken(string refreshToken);
}