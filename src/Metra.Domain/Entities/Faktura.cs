namespace Metra.Domain.Entities;

/// <summary>
/// Faktura (Invoice) asosiy entity
/// </summary>
public class Faktura
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int RentId { get; set; }
    public string RentNumber { get; set; } = string.Empty;
    public string FakturaNumber { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string ResponsibleWorker { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? SkidkaSumma { get; set; }
    public string? SkidkaDescription { get; set; }
    public DateTime Date { get; set; }
    public DateTime RentDate { get; set; }
    public DateTime? DeletedAt { get; set; }
    public List<FakturaDetail> Details { get; set; } = new();
    public List<FakturaFine> Fines { get; set; } = new();
}

/// <summary>
/// Faktura detallari (materiallar ro'yxati)
/// </summary>
public class FakturaDetail
{
    public int Id { get; set; }
    public int Number { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal? MaterialRentPrice { get; set; }
    public string? Period { get; set; }
    public decimal? Summa { get; set; }
}

/// <summary>
/// Faktura jarimalari
/// </summary>
public class FakturaFine
{
    public int Id { get; set; }
    public decimal Summa { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Faktura uchun material
/// </summary>
public class MaterialForFaktura
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Price { get; set; }
    public string? Period { get; set; }
}
