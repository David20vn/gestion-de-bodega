using MiApi.DTOs;

namespace MiApi.Services;

public interface IAuthService
{
    Task<TokenResponse?> Login(LoginRequest request);
}