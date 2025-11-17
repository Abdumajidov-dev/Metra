namespace Metra.Application.DTOs.Responses;

/// <summary>
/// Pagination uchun umumiy javob
/// </summary>
public class PaginatedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int CurrentPage { get; set; }
    public int LastPage { get; set; }
    public int PerPage { get; set; }
    public int Total { get; set; }
    public int From { get; set; }
    public int To { get; set; }
}
