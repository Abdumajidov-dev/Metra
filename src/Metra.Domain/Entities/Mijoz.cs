namespace Metra.Domain.Entities;

/// <summary>
/// Mijoz (Client) entity
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
    public string? Pnfl { get; set; }
    public string? Description { get; set; }
    public string? WhenGiven { get; set; }
    public DateTime? BirthDay { get; set; }
    public string? Image { get; set; }
    public string? ImagePassport { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
