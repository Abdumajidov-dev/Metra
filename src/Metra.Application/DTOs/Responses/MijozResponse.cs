using System.Text.Json.Serialization;

namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Mijoz response (API'dan kelgan ma'lumot)
/// </summary>
public class MijozResponse
{
    // UI uchun
    public int Number { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

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

    [JsonPropertyName("branch_name")]
    public string? BranchName { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    // UI uchun computed properties

    public string PassportDisplay => !string.IsNullOrEmpty(PassportSeries) && !string.IsNullOrEmpty(PassportNumber)
        ? $"{PassportSeries} {PassportNumber}"
        : "-";

    public string ImageUrl => !string.IsNullOrEmpty(Image)
        ? $"http://app.metra-rent.uz/api/public/storage/{Image}"
        : string.Empty;

    public string ImagePassportUrl => !string.IsNullOrEmpty(ImagePassport)
        ? $"http://app.metra-rent.uz/api/public/storage/{ImagePassport}"
        : string.Empty;
}
