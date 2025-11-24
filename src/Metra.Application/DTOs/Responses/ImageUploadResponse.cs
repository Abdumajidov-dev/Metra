using System.Text.Json.Serialization;

namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Rasm yuklash response
/// </summary>
public class ImageUploadResponse
{
    [JsonPropertyName("image_path")]
    public string ImagePath { get; set; } = string.Empty;
}
