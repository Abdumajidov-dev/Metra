using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;

namespace Metra.Application.Services.Interfaces;

/// <summary>
/// Filial boshqarish service interface
/// </summary>
public interface IFilialService
{
    /// <summary>
    /// Barcha filiallarni olish (qidiruv bilan)
    /// </summary>
    /// <param name="search">Qidiruv matni</param>
    /// <returns>Filiallar ro'yxati</returns>
    Task<List<FilialResponse>?> GetAllAsync(string? search = null);

    /// <summary>
    /// Filialni ID bo'yicha olish
    /// </summary>
    /// <param name="filialId">Filial ID</param>
    /// <returns>Filial ma'lumotlari</returns>
    Task<FilialResponse?> GetByIdAsync(int filialId);

    /// <summary>
    /// Filial nomi bo'yicha ID ni olish
    /// </summary>
    /// <param name="name">Filial nomi</param>
    /// <returns>Filial ID</returns>
    Task<int?> GetIdByNameAsync(string name);

    /// <summary>
    /// Yangi filial yaratish
    /// </summary>
    /// <param name="request">Filial ma'lumotlari</param>
    /// <returns>Yaratilgan filial</returns>
    Task<bool> CreateAsync(FilialCreateRequest request);

    /// <summary>
    /// Filialni yangilash
    /// </summary>
    /// <param name="filialId">Filial ID</param>
    /// <param name="request">Yangilangan ma'lumotlar</param>
    /// <returns>Yangilangan filial</returns>
    Task<bool> UpdateAsync(int filialId, FilialUpdateRequest request);

    /// <summary>
    /// Filialni o'chirish
    /// </summary>
    /// <param name="filialId">Filial ID</param>
    /// <returns>Muvaffaqiyatli o'chirildi yoki yo'q</returns>
    Task<bool> DeleteAsync(int filialId);
}
