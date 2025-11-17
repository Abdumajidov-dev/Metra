namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Login javobi uchun DTO
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public UserInfo? User { get; set; }
}

/// <summary>
/// Foydalanuvchi ma'lumotlari
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? FilialId { get; set; }
    public string? FilialName { get; set; }
}
