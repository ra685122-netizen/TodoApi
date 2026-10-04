
namespace TodoApi.Models;
//يمثل البيانات الداخلة للـ endpoint
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}