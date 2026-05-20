// DTOs/TokenResponse.cs
namespace MiApi.DTOs;

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}