using Metra.Application.Configuration;
using Metra.Application.DTOs.Requests;
using Metra.Application.DTOs.Responses;
using Metra.Application.Exceptions;
using Metra.Application.Services.Interfaces.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Metra.Application.Services.Implementation.Base;

/// <summary>
/// Autentifikatsiya servisi
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        HttpClient httpClient,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Tizimga kirish
    /// </summary>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login urinishi: {Phone}", request.Phone);

            // Debug: URL ni log qilish
            var fullUrl = $"{_httpClient.BaseAddress}auth/login";
            _logger.LogInformation("Login URL: {Url}", fullUrl);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var loginData = new
            {
                phone = request.Phone,
                password = request.Password
            };

            var content = new StringContent(
                JsonConvert.SerializeObject(loginData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            // Debug: Response ni log qilish
            _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();

            // Debug: Response content ni tekshirish
            _logger.LogInformation("Response Content (first 200 chars): {Content}",
                responseContent.Length > 200 ? responseContent.Substring(0, 200) : responseContent);

            // HTML qaytarganini tekshirish
            if (responseContent.TrimStart().StartsWith("<"))
            {
                _logger.LogError("Server HTML qaytardi, JSON emas. Base URL yoki endpoint noto'g'ri");
                throw new ApplicationException("Server xatosi: API topilmadi. Iltimos admin bilan bog'laning.");
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = JObject.Parse(responseContent);
                var token = jsonResponse["resoult"]?["token"]?.ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    // Tokenni saqlash
                    _tokenService.SetToken(token);
                    _logger.LogInformation("Login muvaffaqiyatli: {Phone}", request.Phone);

                    return new LoginResponse
                    {
                        Token = token,
                        Success = true
                    };
                }
            }

            // Xatolik holatlarini tekshirish
            switch ((int)response.StatusCode)
            {
                case 401:
                    _logger.LogWarning("Login xato: Noto'g'ri telefon yoki parol");
                    throw new UnauthorizedException("Telefon raqam yoki parol noto'g'ri");

                case 429:
                    _logger.LogWarning("Login xato: Ko'p urinishlar");
                    throw new ApplicationException("Ko'p marta urinish. Iltimos biroz kuting");

                case 403:
                    _logger.LogWarning("Login xato: Ruxsat yo'q");
                    throw new ForbiddenException("Sizda ushbu manbaga kirish uchun ruxsat yo'q");

                case 500:
                    _logger.LogError("Login xato: Server xatolik");
                    throw new ApplicationException("Serverda xatolik yuz berdi");

                default:
                    _logger.LogError("Login xato: {StatusCode} - {Reason}", response.StatusCode, response.ReasonPhrase);
                    throw new ApplicationException($"Xatolik: {response.ReasonPhrase}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Login xato: Network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex) when (ex is not ApplicationException && ex is not UnauthorizedException && ex is not ForbiddenException)
        {
            _logger.LogError(ex, "Login xato: Kutilmagan xatolik");
            throw new ApplicationException("Kutilmagan xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Tizimdan chiqish
    /// </summary>
    public Task LogoutAsync()
    {
        try
        {
            _logger.LogInformation("Logout");
            _tokenService.ClearToken();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout xato");
            throw new ApplicationException("Tizimdan chiqishda xatolik");
        }
    }

    /// <summary>
    /// Joriy foydalanuvchi ma'lumotlarini olish
    /// </summary>
    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token topilmadi");
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/auth/user");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(content);
                _logger.LogInformation("Foydalanuvchi ma'lumotlari olindi");
                return userInfo;
            }

            _logger.LogWarning("Foydalanuvchi ma'lumotlarini olishda xatolik: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Foydalanuvchi ma'lumotlarini olishda network xatolik");
            throw new ApplicationException("Server bilan bog'lanishda xatolik");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Foydalanuvchi ma'lumotlarini olishda xatolik");
            throw new ApplicationException("Kutilmagan xatolik yuz berdi");
        }
    }

    /// <summary>
    /// Foydalanuvchi tizimga kirganmi?
    /// </summary>
    public bool IsAuthenticated()
    {
        try
        {
            var token = _tokenService.GetTokenAsync().GetAwaiter().GetResult();
            return !string.IsNullOrEmpty(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IsAuthenticated xato");
            return false;
        }
    }
}
