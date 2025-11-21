using Newtonsoft.Json;

namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Filial response
/// </summary>
public class FilialResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("responsible_worker")]
    public string? ResponsibleWorker { get; set; }

    [JsonProperty("date")]
    public string? Date { get; set; }

    [JsonProperty("updated_at")]
    public string? UpdatedAt { get; set; }

    // UI uchun qo'shimcha property
    public int Number { get; set; }

    // Type ni ko'rinishda ko'rsatish uchun
    public string TypeDisplay => GetTypeDisplay();

    private string GetTypeDisplay()
    {
        return Type switch
        {
            "main" => "Asosiy",
            "general" => "Asosiy ombor",
            "branch" => "Filial",
            "warehouse" => "Ombor",
            "store" => "Ombor",
            _ => Type
        };
    }
}
