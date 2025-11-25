using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;

namespace Metra.Application.Services.Interfaces.Base;

/// <summary>
/// Avtorizatsiya servisi interfeysi
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Tizimga kirish
    /// </summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Tizimdan chiqish
    /// </summary>
    Task LogoutAsync();

    /// <summary>
    /// Joriy foydalanuvchi ma'lumotlarini olish
    /// </summary>
    Task<UserInfo?> GetCurrentUserAsync();

    /// <summary>
    /// Foydalanuvchi tizimga kirganligini tekshirish
    /// </summary>
    bool IsAuthenticated();
}
