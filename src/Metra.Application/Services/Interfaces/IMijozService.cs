using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;

namespace Metra.Application.Services.Interfaces;

/// <summary>
/// Mijozlar (Clients) uchun service interface
/// </summary>
public interface IMijozService
{
    /// <summary>
    /// Barcha mijozlarni olish (pagination bilan)
    /// </summary>
    Task<PaginatedResult<MijozResponse>?> GetAllAsync(int page = 1, string? search = null, int? branchId = null);

    /// <summary>
    /// Mijozlarni option list sifatida olish (dropdown uchun)
    /// </summary>
    Task<List<MijozResponse>?> GetOptionListAsync(string? clientName = null);

    /// <summary>
    /// Bitta mijozni ID orqali olish
    /// </summary>
    Task<MijozResponse?> GetByIdAsync(int id);

    /// <summary>
    /// Yangi mijoz qo'shish
    /// </summary>
    Task<bool> CreateAsync(MijozCreateRequest request);

    /// <summary>
    /// Mijozni yangilash
    /// </summary>
    Task<bool> UpdateAsync(int id, MijozUpdateRequest request);

    /// <summary>
    /// Mijozni o'chirish
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Rasm yuklash (mijoz yoki pasport rasmi)
    /// </summary>
    Task<string?> UploadImageAsync(string filePath);

    /// <summary>
    /// Rasmni o'chirish
    /// </summary>
    Task<bool> DeleteImageAsync(string imagePath);
}
