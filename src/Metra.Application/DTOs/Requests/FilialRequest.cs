namespace Metra.Application.DTOs.Requests;

/// <summary>
/// Filial yaratish uchun request
/// </summary>
public class FilialCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Filial yangilash uchun request
/// </summary>
public class FilialUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Filial qidiruv uchun request
/// </summary>
public class FilialSearchRequest
{
    public string? BranchName { get; set; }
}
