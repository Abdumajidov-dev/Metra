namespace Metra.Infrastructure.API;

/// <summary>
/// API konfiguratsiyasi
/// </summary>
public static class ApiConfig
{
    /// <summary>
    /// API bazaviy URL
    /// </summary>
    public const string BaseUrl = "http://app.metra-rent.uz/api";

    /// <summary>
    /// Rasm uchun bazaviy URL
    /// </summary>
    public const string ImageBaseUrl = "http://app.metra-rent.uz/api/public/storage/";

    /// <summary>
    /// API timeout (sekundlarda)
    /// </summary>
    public const int TimeoutSeconds = 60;

    /// <summary>
    /// API versiyasi
    /// </summary>
    public const string ApiVersion = "v1";
}
