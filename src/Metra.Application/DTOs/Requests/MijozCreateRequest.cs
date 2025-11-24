using System.Text.Json.Serialization;

namespace Metra.Application.DTOs.Requests;

/// <summary>
/// Yangi mijoz qo'shish uchun request
/// </summary>
public class MijozCreateRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyName("phone_additional")]
    public string? Phone2 { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("passport_series")]
    public string? PassportSeries { get; set; }

    [JsonPropertyName("passport_number")]
    public string? PassportNumber { get; set; }

    [JsonPropertyName("pnfl")]
    public string? Pnfl { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("when_given")]
    public string? WhenGiven { get; set; }

    [JsonPropertyName("birthday")]
    public string? BirthDay { get; set; }

    [JsonPropertyName("image")]
    public string? Image { get; set; }

    [JsonPropertyName("image_pasport")]
    public string? ImagePassport { get; set; }

    [JsonPropertyName("branch_id")]
    public int? BranchId { get; set; }
}
