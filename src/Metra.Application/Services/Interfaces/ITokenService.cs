namespace Metra.Application.Services.Interfaces;

/// <summary>
/// Token boshqaruvi uchun servis interfeysi
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Token ni olish
    /// </summary>
    Task<string?> GetTokenAsync();

    /// <summary>
    /// Token ni saqlash
    /// </summary>
    void SetToken(string token);

    /// <summary>
    /// Token ni o'chirish
    /// </summary>
    void ClearToken();

    /// <summary>
    /// Token mavjudligini tekshirish
    /// </summary>
    bool HasToken();
}
