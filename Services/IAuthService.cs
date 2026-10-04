
using TodoApi.Models;

namespace TodoApi.Services;

public interface IAuthService
{
    TokenResponse? Login(LoginRequest request);

    TokenResponse? RefreshToken(string refreshToken);
}