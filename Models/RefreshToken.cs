
namespace TodoApi.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!; 

    public string UserId { get; set; } = null!; //هذا الـ Refresh Token تابع لأي User؟

    public DateTime ExpiresAt { get; set; }
}