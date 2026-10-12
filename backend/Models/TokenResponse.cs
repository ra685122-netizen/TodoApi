
namespace TodoApi.Models;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;//Access Token

    public string RefreshToken { get; set; } = string.Empty;
}