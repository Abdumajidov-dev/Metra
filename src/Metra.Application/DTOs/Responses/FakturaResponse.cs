using Newtonsoft.Json;

namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Faktura response
/// </summary>
public class FakturaResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("branch_id")]
    public string BranchId { get; set; } = string.Empty;

    [JsonProperty("branch_name")]
    public string BranchName { get; set; } = string.Empty;

    [JsonProperty("client_id")]
    public string ClientId { get; set; } = string.Empty;

    [JsonProperty("client_name")]
    public string ClientName { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("responsible_worker")]
    public string ResponsibleWorker { get; set; } = string.Empty;

    [JsonProperty("rent_id")]
    public int RentId { get; set; }

    [JsonProperty("rent_number")]
    public string RentNumber { get; set; } = string.Empty;

    [JsonProperty("rent_date")]
    public string RentDate { get; set; } = string.Empty;

    [JsonProperty("payment_status")]
    public string PaymentStatus { get; set; } = string.Empty;

    [JsonProperty("faktura_number")]
    public string FakturaNumber { get; set; } = string.Empty;

    [JsonProperty("skidka_summa")]
    public decimal? SkidkaSumma { get; set; }

    [JsonProperty("deleted_at")]
    public string? DeletedAt { get; set; }

    [JsonProperty("skidka_description")]
    public string? SkidkaDescription { get; set; }

    [JsonProperty("date")]
    public string Date { get; set; } = string.Empty;

    [JsonProperty("details")]
    public List<FakturaDetailResponse> Details { get; set; } = new();

    [JsonProperty("fines")]
    public List<FakturaFineResponse> Fines { get; set; } = new();
}

/// <summary>
/// Faktura detail response
/// </summary>
public class FakturaDetailResponse
{
    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("material_id")]
    public int MaterialId { get; set; }

    [JsonProperty("material_name")]
    public string MaterialName { get; set; } = string.Empty;

    [JsonProperty("unit_name")]
    public string UnitName { get; set; } = string.Empty;

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("material_rent_price")]
    public decimal? MaterialRentPrice { get; set; }

    [JsonProperty("period")]
    public string? Period { get; set; }

    [JsonProperty("summa")]
    public decimal? Summa { get; set; }
}

/// <summary>
/// Faktura fine response
/// </summary>
public class FakturaFineResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("summa")]
    public decimal Summa { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Material for faktura response
/// </summary>
public class MaterialForFakturaResponse
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("material_id")]
    public int MaterialId { get; set; }

    [JsonProperty("material_name")]
    public string MaterialName { get; set; } = string.Empty;

    [JsonProperty("unit_name")]
    public string UnitName { get; set; } = string.Empty;

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("price")]
    public decimal Price { get; set; }

    [JsonProperty("period")]
    public string? Period { get; set; }
}
