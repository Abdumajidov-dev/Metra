using Metra.Application.Configuration;
using Metra.Application.DTOs.Requests.Malumotlar;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Exceptions;
using Metra.Application.Services.Interfaces.Base;
using Metra.Application.Services.Interfaces.Malumotlar;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Metra.Application.Services.Implementation.Malumotlar;

/// <summary>
/// Filial boshqarish service implementatsiyasi
/// </summary>
public class FilialService : IFilialService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<FilialService> _logger;

    public FilialService(
        HttpClient httpClient,
        ITokenService tokenService,
        ILogger<FilialService> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Barcha filiallarni olish (qidiruv bilan)
    /// </summary>
    public async Task<List<FilialResponse>?> GetAllAsync(string? search = null)
    {
        try
        {
            _logger.LogInformation("Filiallar yuklanmoqda: Search={Search}", search);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            // Token diagnostikasi (faqat boshini ko'rsatamiz)
            var tokenPreview = token.Length > 20 ? $"{token.Substring(0, 20)}..." : token;
            _logger.LogDebug("Token: {TokenPreview}", tokenPreview);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // URL diagnostikasi
            var fullUrl = $"{_httpClient.BaseAddress}branches";
            _logger.LogInformation("Request URL: {Url}", fullUrl);

            // Request body - eski versiya kabi, har doim branch_name yuboriladi
            var requestBody = new
            {
                branch_name = search ?? string.Empty
            };

            _logger.LogDebug("Request: POST branches with branch_name={BranchName}",
                string.IsNullOrEmpty(search) ? "(bo'sh)" : search);

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("branches", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("API Response: StatusCode={StatusCode}, ContentType={ContentType}, ContentLength={Length}",
                response.StatusCode,
                response.Content.Headers.ContentType?.MediaType ?? "null",
                responseContent.Length);

            _logger.LogDebug("Response Content (first 500 chars): {Content}",
                responseContent.Substring(0, Math.Min(500, responseContent.Length)));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Filiallar yuklanmadi: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, responseContent);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedException("Tizimga qayta kiring");
                }

                throw new ApplicationException($"Filiallar yuklanishda xatolik: {response.StatusCode}");
            }

            // JSON parse qilish
            try
            {
                // Agar response HTML yoki XML bo'lsa, darhol xatolik
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("HTML/XML javob keldi JSON o'rniga: {Response}",
                        responseContent.Substring(0, Math.Min(200, responseContent.Length)));
                    throw new ApplicationException("Server HTML qaytardi, JSON kutilgan edi");
                }

                var jsonResponse = JObject.Parse(responseContent);
                var filials = jsonResponse["resoult"]?.ToObject<List<FilialResponse>>();

                _logger.LogInformation("Filiallar muvaffaqiyatli yuklandi: Count={Count}", filials?.Count ?? 0);
                return filials;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse xatolik: Response={Response}",
                    responseContent.Substring(0, Math.Min(500, responseContent.Length)));
                throw new ApplicationException("Server javobini o'qishda xatolik");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Filiallar yuklanishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException)
        {
            _logger.LogError(ex, "Filiallar yuklanishda xatolik");
            throw new ApplicationException("Filiallar yuklanishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Filialni ID bo'yicha olish
    /// </summary>
    public async Task<FilialResponse?> GetByIdAsync(int filialId)
    {
        try
        {
            _logger.LogInformation("Filial yuklanmoqda: Id={FilialId}", filialId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"branches/{filialId}");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response: StatusCode={StatusCode}, Content={Content}",
                response.StatusCode, content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Filial topilmadi: Id={Id}", filialId);
                throw new NotFoundException($"Filial topilmadi (ID: {filialId})");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Tizimga qayta kiring");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Filial yuklanmadi: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, content);
                throw new ApplicationException($"Filial yuklanishda xatolik: {response.StatusCode}");
            }

            try
            {
                // Agar response HTML yoki XML bo'lsa, darhol xatolik
                if (content.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("HTML/XML javob keldi JSON o'rniga: {Response}",
                        content.Substring(0, Math.Min(200, content.Length)));
                    throw new ApplicationException("Server HTML qaytardi, JSON kutilgan edi");
                }

                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    var filial = jsonResponse["resoult"]?.ToObject<FilialResponse>();
                    _logger.LogInformation("Filial muvaffaqiyatli yuklandi: Id={Id}", filialId);
                    return filial;
                }
                else
                {
                    _logger.LogWarning("Filial topilmadi: Id={Id}", filialId);
                    return null;
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse xatolik: Response={Response}",
                    content.Substring(0, Math.Min(500, content.Length)));
                throw new ApplicationException("Server javobini o'qishda xatolik");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Filial yuklanishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Filial yuklanishda xatolik");
            throw new ApplicationException("Filial yuklanishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Filial nomi bo'yicha ID ni olish
    /// </summary>
    public async Task<int?> GetIdByNameAsync(string name)
    {
        try
        {
            _logger.LogInformation("Filial ID qidirilmoqda: Name={Name}", name);

            var filials = await GetAllAsync(name);

            if (filials != null && filials.Count > 0)
            {
                var foundFilial = filials.FirstOrDefault(f =>
                    f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                if (foundFilial != null)
                {
                    _logger.LogInformation("Filial topildi: Id={Id}, Name={Name}",
                        foundFilial.Id, foundFilial.Name);
                    return foundFilial.Id;
                }
            }

            _logger.LogWarning("Filial topilmadi: Name={Name}", name);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Filial ID qidirishda xatolik: Name={Name}", name);
            throw new ApplicationException("Filial qidirishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Yangi filial yaratish
    /// </summary>
    public async Task<bool> CreateAsync(FilialCreateRequest request)
    {
        try
        {
            _logger.LogInformation("Yangi filial yaratilmoqda: Name={Name}", request.Name);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                name = request.Name,
                description = request.Description,
                type = request.Type
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("branch", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response: StatusCode={StatusCode}, Content={Content}",
                response.StatusCode, responseContent);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Tizimga qayta kiring");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Filial yaratilmadi: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, responseContent);
                throw new ApplicationException($"Filial yaratishda xatolik: {response.StatusCode}");
            }

            try
            {
                // Agar response HTML yoki XML bo'lsa, darhol xatolik
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("HTML/XML javob keldi JSON o'rniga: {Response}",
                        responseContent.Substring(0, Math.Min(200, responseContent.Length)));
                    throw new ApplicationException("Server HTML qaytardi, JSON kutilgan edi");
                }

                var jsonResponse = JObject.Parse(responseContent);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Filial muvaffaqiyatli yaratildi: Name={Name}", request.Name);
                    return true;
                }
                else
                {
                    var message = jsonResponse["message"]?.ToString() ?? "Noma'lum xatolik";
                    _logger.LogWarning("Filial yaratilmadi: {Message}", message);
                    throw new ApplicationException($"Filial yaratilmadi: {message}");
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse xatolik: Response={Response}",
                    responseContent.Substring(0, Math.Min(500, responseContent.Length)));
                throw new ApplicationException("Server javobini o'qishda xatolik");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Filial yaratishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException)
        {
            _logger.LogError(ex, "Filial yaratishda xatolik");
            throw new ApplicationException("Filial yaratishda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Filialni yangilash
    /// </summary>
    public async Task<bool> UpdateAsync(int filialId, FilialUpdateRequest request)
    {
        try
        {
            _logger.LogInformation("Filial yangilan moqda: Id={FilialId}, Name={Name}",
                filialId, request.Name);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                name = request.Name,
                description = request.Description,
                type = request.Type
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"branch/{filialId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response: StatusCode={StatusCode}, Content={Content}",
                response.StatusCode, responseContent);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Filial topilmadi: Id={Id}", filialId);
                throw new NotFoundException($"Filial topilmadi (ID: {filialId})");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Tizimga qayta kiring");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Filial yangilanmadi: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, responseContent);
                throw new ApplicationException($"Filial yangilashda xatolik: {response.StatusCode}");
            }

            try
            {
                // Agar response HTML yoki XML bo'lsa, darhol xatolik
                if (responseContent.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("HTML/XML javob keldi JSON o'rniga: {Response}",
                        responseContent.Substring(0, Math.Min(200, responseContent.Length)));
                    throw new ApplicationException("Server HTML qaytardi, JSON kutilgan edi");
                }

                var jsonResponse = JObject.Parse(responseContent);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Filial muvaffaqiyatli yangilandi: Id={Id}", filialId);
                    return true;
                }
                else
                {
                    var message = jsonResponse["message"]?.ToString() ?? "Noma'lum xatolik";
                    _logger.LogWarning("Filial yangilanmadi: {Message}", message);
                    throw new ApplicationException($"Filial yangilanmadi: {message}");
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse xatolik: Response={Response}",
                    responseContent.Substring(0, Math.Min(500, responseContent.Length)));
                throw new ApplicationException("Server javobini o'qishda xatolik");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Filial yangilashda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Filial yangilashda xatolik");
            throw new ApplicationException("Filial yangilashda xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Filialni o'chirish
    /// </summary>
    public async Task<bool> DeleteAsync(int filialId)
    {
        try
        {
            _logger.LogInformation("Filial o'chirilmoqda: Id={FilialId}", filialId);

            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                throw new UnauthorizedException("Tizimga kiring");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"branch/delete/{filialId}");
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogDebug("API Response: StatusCode={StatusCode}, Content={Content}",
                response.StatusCode, content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Filial topilmadi: Id={Id}", filialId);
                throw new NotFoundException($"Filial topilmadi (ID: {filialId})");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException("Tizimga qayta kiring");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Filial o'chirilmadi: StatusCode={StatusCode}, Response={Response}",
                    response.StatusCode, content);
                throw new ApplicationException($"Filial o'chirishda xatolik: {response.StatusCode}");
            }

            try
            {
                // Agar response HTML yoki XML bo'lsa, darhol xatolik
                if (content.TrimStart().StartsWith("<"))
                {
                    _logger.LogError("HTML/XML javob keldi JSON o'rniga: {Response}",
                        content.Substring(0, Math.Min(200, content.Length)));
                    throw new ApplicationException("Server HTML qaytardi, JSON kutilgan edi");
                }

                var jsonResponse = JObject.Parse(content);

                if (jsonResponse["success"]?.Value<bool>() == true)
                {
                    _logger.LogInformation("Filial muvaffaqiyatli o'chirildi: Id={Id}", filialId);
                    return true;
                }
                else
                {
                    var message = jsonResponse["message"]?.ToString() ?? "Noma'lum xatolik";
                    _logger.LogWarning("Filial o'chirilmadi: {Message}", message);
                    throw new ApplicationException($"Filial o'chirilmadi: {message}");
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON parse xatolik: Response={Response}",
                    content.Substring(0, Math.Min(500, content.Length)));
                throw new ApplicationException("Server javobini o'qishda xatolik");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Filial o'chirishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not NotFoundException)
        {
            _logger.LogError(ex, "Filial o'chirishda xatolik");
            throw new ApplicationException("Filial o'chirishda xatolik yuz berdi");
        }
    }
}
