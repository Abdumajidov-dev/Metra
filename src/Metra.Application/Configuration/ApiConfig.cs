namespace Metra.Application.Configuration;

/// <summary>
/// API konfiguratsiyasi
/// </summary>
public static class ApiConfig
{
    /// <summary>
    /// API bazaviy URL (oxirida / bo'lishi kerak!)
    /// </summary>
    //public const string BaseUrl = "http://app.metra-rent.uz/api/";
    public const string BaseUrl = "http://10.100.104.104:4001/api/";
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
