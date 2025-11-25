using Metra.Application.Configuration;
using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;
using Metra.Application.Exceptions;
using Metra.Application.Services.Interfaces;
using Metra.Application.Services.Interfaces.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Metra.Application.Services.Implementation;

/// <summary>
/// Faktura boshqarish service implementatsiyasi
/// </summary>
public class FakturaService : IFakturaService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<FakturaService> _logger;

    public FakturaService(
        HttpClient httpClient,
        ITokenService tokenService,
        ILogger<FakturaService> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Faktura uchun materiallar ro'yxatini olish
    /// </summary>
    public async Task<List<MaterialForFakturaResponse>?> GetMaterialsForFakturaAsync(int rentId)
    {
        try
        {
            _logger.LogInformation("Faktura uchun materiallar yuklanmoqda: RentId={RentId}", rentId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/faktura-return/show/materials/{rentId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    var materials = jsonResponse["resoult"]?.ToObject<List<MaterialForFakturaResponse>>();
                    _logger.LogInformation("Materiallar muvaffaqiyatli yuklandi: Count={Count}", materials?.Count ?? 0);
                    return materials;
                }
                else
                {
                    _logger.LogWarning("Materiallar yuklanmadi: success=false");
                    return null;
                }
            }

            _logger.LogWarning("Materiallar yuklanmadi: StatusCode={StatusCode}", response.StatusCode);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Materiallar yuklanishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException)
        {
            _logger.LogError(ex, "Materiallar yuklanishda xatolik");
            throw new ApplicationException("Materiallar yuklanishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Fakturani ID bo'yicha olish
    /// </summary>
    public async Task<FakturaResponse?> GetByIdAsync(int fakturaId)
    {
        try
        {
            _logger.LogInformation("Faktura yuklanmoqda: Id={FakturaId}", fakturaId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/faktura-return/{fakturaId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);
                var resultToken = jsonResponse["resoult"];

                if (resultToken != null)
                {
                    var faktura = resultToken.ToObject<FakturaResponse>();

                    // skidka_summa ni alohida parse qilamiz (null bo'lishi mumkin)
                    var rawSkidka = resultToken["skidka_summa"];
                    if (rawSkidka != null && (rawSkidka.Type == JTokenType.Float ||
                                              rawSkidka.Type == JTokenType.Integer ||
                                              rawSkidka.Type == JTokenType.String))
                    {
                        faktura!.SkidkaSumma = rawSkidka.Value<decimal?>();
                    }
                    else
                    {
                        faktura!.SkidkaSumma = null;
                    }

                    _logger.LogInformation("Faktura muvaffaqiyatli yuklandi: Id={Id}", fakturaId);
                    return faktura;
                }

                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                return null;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                throw new NotFoundException($"Faktura topilmadi (ID: {fakturaId})");
            }

            _logger.LogWarning("Faktura yuklanmadi: StatusCode={StatusCode}", response.StatusCode);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura yuklanishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Faktura yuklanishda xatolik");
            throw new ApplicationException("Faktura yuklanishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Yangi faktura yaratish
    /// </summary>
    public async Task<FakturaResponse?> CreateAsync(FakturaRequest request)
    {
        try
        {
            _logger.LogInformation("Yangi faktura yaratilmoqda: ClientId={ClientId}, RentId={RentId}",
                request.ClientId, request.RentId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                branch_id = request.BranchId.ToString(),
                client_id = request.ClientId.ToString(),
                rent_id = request.RentId,
                description = request.Description,
                skidka_summa = request.SkidkaSumma,
                skidka_description = request.SkidkaDescription,
                details = request.Details.Select(d => new
                {
                    material_id = d.MaterialId,
                    count = d.Count,
                    period = d.Period,
                    summa = d.Summa,
                    material_rent_price = d.MaterialRentPrice
                }).ToList(),
                fines = request.Fines.Select(f => new
                {
                    id = (int?)null,
                    summa = f.Summa,
                    description = f.Description
                }).ToList()
            };

            var json = JsonConvert.SerializeObject(body, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/faktura-return", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                var faktura = jsonResponse["resoult"]?.ToObject<FakturaResponse>();

                _logger.LogInformation("Faktura muvaffaqiyatli yaratildi: Id={Id}", faktura?.Id);
                return faktura;
            }

            _logger.LogWarning("Faktura yaratilmadi: StatusCode={StatusCode}", response.StatusCode);
            throw new ApplicationException("Faktura yaratishda xatolik yuz berdi");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura yaratishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException)
        {
            _logger.LogError(ex, "Faktura yaratishda xatolik");
            throw new ApplicationException("Faktura yaratishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Fakturani yangilash
    /// </summary>
    public async Task<FakturaResponse?> UpdateAsync(int fakturaId, FakturaUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Faktura yangilan moqda: Id={FakturaId}", fakturaId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var body = new
            {
                id = fakturaId,
                branch_id = request.BranchId,
                branch_name = request.BranchName,
                client_id = request.ClientId,
                client_name = request.ClientName,
                date = request.Date,
                rent_date = request.RentDate,
                rent_id = request.RentId,
                rent_number = request.RentNumber,
                faktura_number = request.FakturaNumber,
                payment_status = request.PaymentStatus,
                responsible_worker = request.ResponsibleWorker,
                description = request.Description,
                skidka_summa = request.SkidkaSumma,
                skidka_description = request.SkidkaDescription,
                deleted_at = request.DeletedAt,
                delete_list = request.DeleteList,
                details = request.Details.Select(d => new
                {
                    id = d.Id,
                    material_id = d.MaterialId,
                    material_name = d.MaterialName,
                    unit_name = d.UnitName,
                    count = d.Count,
                    period = d.Period,
                    summa = d.Summa,
                    material_rent_price = d.MaterialRentPrice
                }).ToList(),
                fines = request.Fines.Select(f => new
                {
                    id = f.Id,
                    summa = f.Summa,
                    description = f.Description
                }).ToList()
            };

            var json = JsonConvert.SerializeObject(body, Formatting.Indented);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"/faktura-return/{fakturaId}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseContent);
                var faktura = jsonResponse["resoult"]?.ToObject<FakturaResponse>();

                _logger.LogInformation("Faktura muvaffaqiyatli yangilandi: Id={Id}", fakturaId);
                return faktura;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                throw new NotFoundException($"Faktura topilmadi (ID: {fakturaId})");
            }

            _logger.LogWarning("Faktura yangilanmadi: StatusCode={StatusCode}", response.StatusCode);
            throw new ApplicationException("Faktura yangilashda xatolik yuz berdi");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura yangilashda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Faktura yangilashda xatolik");
            throw new ApplicationException("Faktura yangilashda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Fakturani o'chirish (soft delete)
    /// </summary>
    public async Task<bool> DeleteAsync(int fakturaId)
    {
        try
        {
            _logger.LogInformation("Faktura o'chirilmoqda: Id={FakturaId}", fakturaId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/faktura-return/delete/{fakturaId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Faktura muvaffaqiyatli o'chirildi: Id={Id}", fakturaId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Faktura o'chirilmadi: success=false, Id={Id}", fakturaId);
                    return false;
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                throw new NotFoundException($"Faktura topilmadi (ID: {fakturaId})");
            }

            _logger.LogWarning("Faktura o'chirilmadi: StatusCode={StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura o'chirishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Faktura o'chirishda xatolik");
            throw new ApplicationException("Faktura o'chirishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Fakturani butunlay o'chirish (hard delete)
    /// </summary>
    public async Task<bool> ForceDeleteAsync(int fakturaId)
    {
        try
        {
            _logger.LogInformation("Faktura butunlay o'chirilmoqda: Id={FakturaId}", fakturaId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/faktura-return/force-delete/{fakturaId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Faktura butunlay o'chirildi: Id={Id}", fakturaId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Faktura butunlay o'chirilmadi: success=false, Id={Id}", fakturaId);
                    return false;
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                throw new NotFoundException($"Faktura topilmadi (ID: {fakturaId})");
            }

            _logger.LogWarning("Faktura butunlay o'chirilmadi: StatusCode={StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura butunlay o'chirishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Faktura butunlay o'chirishda xatolik");
            throw new ApplicationException("Faktura butunlay o'chirishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// O'chirilgan fakturani tiklash
    /// </summary>
    public async Task<bool> RestoreAsync(int fakturaId)
    {
        try
        {
            _logger.LogInformation("Faktura tiklanmoqda: Id={FakturaId}", fakturaId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/faktura-return/restore/{fakturaId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Faktura muvaffaqiyatli tiklandi: Id={Id}", fakturaId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Faktura tiklanmadi: success=false, Id={Id}", fakturaId);
                    return false;
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Faktura topilmadi: Id={Id}", fakturaId);
                throw new NotFoundException($"Faktura topilmadi (ID: {fakturaId})");
            }

            _logger.LogWarning("Faktura tiklanmadi: StatusCode={StatusCode}", response.StatusCode);
            return false;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Faktura tiklashda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Faktura tiklashda xatolik");
            throw new ApplicationException("Faktura tiklashda xatolik yuz berdi");
        }
    }
}
