namespace Metra.Application.DTOs.Requests;

/// <summary>
/// Faktura yaratish uchun request
/// </summary>
public class FakturaRequest
{
    public int BranchId { get; set; }
    public int ClientId { get; set; }
    public int RentId { get; set; }
    public string? Description { get; set; }
    public decimal? SkidkaSumma { get; set; }
    public string? SkidkaDescription { get; set; }
    public List<FakturaDetailRequest> Details { get; set; } = new();
    public List<FakturaFineRequest> Fines { get; set; } = new();
}

/// <summary>
/// Faktura yangilash uchun request
/// </summary>
public class FakturaUpdateRequest
{
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string RentDate { get; set; } = string.Empty;
    public int RentId { get; set; }
    public string RentNumber { get; set; } = string.Empty;
    public string FakturaNumber { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string ResponsibleWorker { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? SkidkaSumma { get; set; }
    public string? SkidkaDescription { get; set; }
    public string? DeletedAt { get; set; }
    public List<int> DeleteList { get; set; } = new();
    public List<FakturaDetailRequest> Details { get; set; } = new();
    public List<FakturaFineRequest> Fines { get; set; } = new();
}

/// <summary>
/// Faktura detail (material) request
/// </summary>
public class FakturaDetailRequest
{
    public int? Id { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public int Count { get; set; }
    public string? Period { get; set; }
    public decimal? Summa { get; set; }
    public decimal? MaterialRentPrice { get; set; }
}

/// <summary>
/// Faktura jarima request
/// </summary>
public class FakturaFineRequest
{
    public int? Id { get; set; }
    public decimal Summa { get; set; }
    public string Description { get; set; } = string.Empty;
}
