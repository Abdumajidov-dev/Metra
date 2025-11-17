namespace Metra.Application.DTOs.Requests;

/// <summary>
/// Login so'rovi uchun DTO
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
