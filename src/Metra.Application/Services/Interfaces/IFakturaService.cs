using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;

namespace Metra.Application.Services.Interfaces;

/// <summary>
/// Faktura boshqarish service interface
/// </summary>
public interface IFakturaService
{
    /// <summary>
    /// Faktura uchun materiallar ro'yxatini olish
    /// </summary>
    /// <param name="rentId">Shartnoma ID</param>
    /// <returns>Materiallar ro'yxati</returns>
    Task<List<MaterialForFakturaResponse>?> GetMaterialsForFakturaAsync(int rentId);

    /// <summary>
    /// Fakturani ID bo'yicha olish
    /// </summary>
    /// <param name="fakturaId">Faktura ID</param>
    /// <returns>Faktura ma'lumotlari</returns>
    Task<FakturaResponse?> GetByIdAsync(int fakturaId);

    /// <summary>
    /// Yangi faktura yaratish
    /// </summary>
    /// <param name="request">Faktura ma'lumotlari</param>
    /// <returns>Yaratilgan faktura</returns>
    Task<FakturaResponse?> CreateAsync(FakturaRequest request);

    /// <summary>
    /// Fakturani yangilash
    /// </summary>
    /// <param name="fakturaId">Faktura ID</param>
    /// <param name="request">Yangilangan ma'lumotlar</param>
    /// <returns>Yangilangan faktura</returns>
    Task<FakturaResponse?> UpdateAsync(int fakturaId, FakturaUpdateRequest request);

    /// <summary>
    /// Fakturani o'chirish (soft delete)
    /// </summary>
    /// <param name="fakturaId">Faktura ID</param>
    /// <returns>Muvaffaqiyatli o'chirildi yoki yo'q</returns>
    Task<bool> DeleteAsync(int fakturaId);

    /// <summary>
    /// Fakturani butunlay o'chirish (hard delete)
    /// </summary>
    /// <param name="fakturaId">Faktura ID</param>
    /// <returns>Muvaffaqiyatli o'chirildi yoki yo'q</returns>
    Task<bool> ForceDeleteAsync(int fakturaId);

    /// <summary>
    /// O'chirilgan fakturani tiklash
    /// </summary>
    /// <param name="fakturaId">Faktura ID</param>
    /// <returns>Muvaffaqiyatli tiklandi yoki yo'q</returns>
    Task<bool> RestoreAsync(int fakturaId);
}
