using System.Text.Json.Serialization;

namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Login javobi uchun DTO
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Login muvaffaqiyatlimi?
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// JWT token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Foydalanuvchi ma'lumotlari
    /// </summary>
    public UserInfo? UserInfo { get; set; }
}

/// <summary>
/// Foydalanuvchi ma'lumotlari
/// </summary>
public class UserInfo
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    [JsonPropertyName("branch_id")]
    public int BranchId { get; set; }
    [JsonPropertyName("branch_name")]
    public string BranchName { get; set; } = string.Empty;
    [JsonPropertyName("branch_type")]
    public string BranchType { get; set; } = string.Empty;
    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = new();
    [JsonPropertyName("permissions")]
    public List<string> Permissions { get; set; } = new();
}
