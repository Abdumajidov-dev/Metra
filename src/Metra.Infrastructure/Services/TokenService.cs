using Metra.Application.Services.Interfaces.Base;
using Metra.Infrastructure.Persistence.Settings;

namespace Metra.Infrastructure.Services;

/// <summary>
/// Token servisi implementatsiyasi
/// </summary>
public class TokenService : ITokenService
{
    private const string TokenKey = "auth_token";
    private readonly AppSettings _settings;
    private string? _cachedToken;

    public TokenService(AppSettings settings)
    {
        _settings = settings;
    }

    public Task<string?> GetTokenAsync()
    {
        if (_cachedToken != null)
        {
            return Task.FromResult<string?>(_cachedToken);
        }

        _cachedToken = _settings.GetSetting<string>(TokenKey);
        return Task.FromResult<string?>(_cachedToken);
    }

    public void SetToken(string token)
    {
        _cachedToken = token;
        _settings.SetSetting(TokenKey, token);
    }

    public void ClearToken()
    {
        _cachedToken = null;
        _settings.RemoveSetting(TokenKey);
    }

    public bool HasToken()
    {
        return !string.IsNullOrEmpty(_cachedToken) ||
               !string.IsNullOrEmpty(_settings.GetSetting<string>(TokenKey));
    }
}
