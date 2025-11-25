using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Metra.Application.Configuration;
using Metra.Application.DTOs.Requests.Malumotlar;
using Metra.Application.DTOs.Responses;
using Metra.Application.DTOs.Responses.Malumotlar;
using Metra.Application.Services.Interfaces;
using Metra.Application.Services.Interfaces.Base;
using Microsoft.Extensions.Logging;

namespace Metra.Application.Services.Implementation.Malumotlar;

/// <summary>
/// Mijozlar (Clients) service implementation
/// </summary>
public class MijozService : IMijozService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<MijozService> _logger;

    public MijozService(
        HttpClient httpClient,
        ITokenService tokenService,
        ILogger<MijozService> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Barcha mijozlarni olish (pagination bilan)
    /// </summary>
    public async Task<PaginatedResult<MijozResponse>?> GetAllAsync(int page = 1, string? search = null, int? branchId = null)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                client_name = search,
                branch_id = branchId
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"clients?page={page}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            var root = jsonDocument.RootElement;

            if (root.TryGetProperty("resoult", out var resultElement))
            {
                var dataArray = resultElement.GetProperty("data");
                var data = JsonSerializer.Deserialize<List<MijozResponse>>(dataArray.GetRawText());

                var meta = resultElement.GetProperty("meta");
                var currentPage = meta.GetProperty("current_page").GetInt32();
                var lastPage = meta.GetProperty("last_page").GetInt32();
                var total = meta.GetProperty("total").GetInt32();
                var perPage = meta.GetProperty("per_page").GetInt32();
                var from = meta.GetProperty("from").GetInt32();
                var to = meta.GetProperty("to").GetInt32();

                return new PaginatedResult<MijozResponse>
                {
                    Data = data ?? new List<MijozResponse>(),
                    CurrentPage = currentPage,
                    LastPage = lastPage,
                    Total = total,
                    PerPage = perPage,
                    From = from,
                    To = to
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozlarni yuklashda xatolik");
            throw new ApplicationException("Mijozlarni yuklashda xatolik yuz berdi", ex);
        }
    }

    /// <summary>
    /// Mijozlarni option list sifatida olish (dropdown uchun)
    /// </summary>
    public async Task<List<MijozResponse>?> GetOptionListAsync(string? clientName = null)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                client_name = clientName
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/client/option/lists", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResultNotPagination<List<MijozResponse>>>(responseContent);

            return result?.Success == true ? result.Result : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozlar option list yuklashda xatolik");
            throw new ApplicationException("Mijozlar ro'yxatini yuklashda xatolik yuz berdi", ex);
        }
    }

    /// <summary>
    /// Bitta mijozni ID orqali olish
    /// </summary>
    public async Task<MijozResponse?> GetByIdAsync(int id)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/client/{id}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResultNotPagination<MijozResponse>>(responseContent);

            return result?.Success == true ? result.Result : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozni yuklashda xatolik: {Id}", id);
            throw new ApplicationException($"Mijozni yuklashda xatolik yuz berdi (ID: {id})", ex);
        }
    }

    /// <summary>
    /// Yangi mijoz qo'shish
    /// </summary>
    public async Task<bool> CreateAsync(MijozRequest request)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/client", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            var success = jsonDocument.RootElement.GetProperty("success").GetBoolean();

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijoz qo'shishda xatolik");
            throw new ApplicationException("Mijoz qo'shishda xatolik yuz berdi", ex);
        }
    }

    /// <summary>
    /// Mijozni yangilash
    /// </summary>
    public async Task<bool> UpdateAsync(int id, MijozUpdateRequest request)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"/client/{id}", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            var success = jsonDocument.RootElement.GetProperty("success").GetBoolean();

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozni yangilashda xatolik: {Id}", id);
            throw new ApplicationException($"Mijozni yangilashda xatolik yuz berdi (ID: {id})", ex);
        }
    }

    /// <summary>
    /// Mijozni o'chirish
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"/client/delete/{id}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            var success = jsonDocument.RootElement.GetProperty("success").GetBoolean();

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mijozni o'chirishda xatolik: {Id}", id);
            throw new ApplicationException($"Mijozni o'chirishda xatolik yuz berdi (ID: {id})", ex);
        }
    }

    /// <summary>
    /// Rasm yuklash (mijoz yoki pasport rasmi)
    /// </summary>
    public async Task<string?> UploadImageAsync(string filePath)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var form = new MultipartFormDataContent();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var safeFileName = Uri.EscapeDataString(Path.GetFileName(filePath));
            form.Add(fileContent, "file", safeFileName);

            var response = await _httpClient.PostAsync("/client/image/store", form);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ResultNotPagination<ImageUploadResponse>>(responseContent);

            if (result?.Success == true)
            {
                return result.Result.ImagePath;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rasm yuklashda xatolik: {FilePath}", filePath);
            throw new ApplicationException("Rasm yuklashda xatolik yuz berdi", ex);
        }
    }

    /// <summary>
    /// Rasmni o'chirish
    /// </summary>
    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var safeFilePath = Uri.EscapeDataString(imagePath);
            var response = await _httpClient.DeleteAsync($"/client/image/delete?file={safeFilePath}");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseContent);
            var success = jsonDocument.RootElement.GetProperty("success").GetBoolean();

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rasmni o'chirishda xatolik: {ImagePath}", imagePath);
            throw new ApplicationException("Rasmni o'chirishda xatolik yuz berdi", ex);
        }
    }
}
