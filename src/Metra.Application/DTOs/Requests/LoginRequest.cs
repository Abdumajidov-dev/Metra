using System.Text.Json.Serialization;

namespace Metra.Application.DTOs.Requests;

/// <summary>
/// Login so'rovi uchun DTO
/// </summary>
public class LoginRequest
{
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
