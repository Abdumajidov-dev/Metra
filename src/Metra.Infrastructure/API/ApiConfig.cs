// ESLATMA: Bu fayl eski, yangi ApiConfig Metra.Application.Configuration dan foydalaning
// Bu faylni backward compatibility uchun saqlab qoldik

using Metra.Application.Configuration;

namespace Metra.Infrastructure.API;

/// <summary>
/// API konfiguratsiyasi (Eski - Metra.Application.Configuration.ApiConfig ishlatilsin)
/// </summary>
[Obsolete("Metra.Application.Configuration.ApiConfig ishlatilsin")]
public static class ApiConfig
{
    /// <summary>
    /// API bazaviy URL
    /// </summary>
    public const string BaseUrl = Application.Configuration.ApiConfig.BaseUrl;

    /// <summary>
    /// Rasm uchun bazaviy URL
    /// </summary>
    public const string ImageBaseUrl = Application.Configuration.ApiConfig.ImageBaseUrl;

    /// <summary>
    /// API timeout (sekundlarda)
    /// </summary>
    public const int TimeoutSeconds = Application.Configuration.ApiConfig.TimeoutSeconds;

    /// <summary>
    /// API versiyasi
    /// </summary>
    public const string ApiVersion = Application.Configuration.ApiConfig.ApiVersion;
}
