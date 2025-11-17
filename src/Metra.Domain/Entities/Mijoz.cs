namespace Metra.Domain.Entities;

/// <summary>
/// Mijoz entity
/// </summary>
public class Mijoz
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? PassportSeria { get; set; }
    public string? PassportNumber { get; set; }
    public int? FilialId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
