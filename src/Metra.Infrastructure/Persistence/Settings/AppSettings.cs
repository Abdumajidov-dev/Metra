using System.Text.Json;

namespace Metra.Infrastructure.Persistence.Settings;

/// <summary>
/// Ilova sozlamalari
/// </summary>
public class AppSettings
{
    private static readonly string SettingsFilePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Metra", "settings.json");

    private Dictionary<string, object> _settings = new();

    public AppSettings()
    {
        LoadSettings();
    }

    /// <summary>
    /// Sozlamani olish
    /// </summary>
    public T? GetSetting<T>(string key, T? defaultValue = default)
    {
        if (_settings.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }
            return (T?)value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Sozlamani saqlash
    /// </summary>
    public void SetSetting<T>(string key, T value)
    {
        _settings[key] = value!;
        SaveSettings();
    }

    /// <summary>
    /// Sozlamani o'chirish
    /// </summary>
    public void RemoveSetting(string key)
    {
        _settings.Remove(key);
        SaveSettings();
    }

    /// <summary>
    /// Sozlamalarni yuklash
    /// </summary>
    private void LoadSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                var json = File.ReadAllText(SettingsFilePath);
                _settings = JsonSerializer.Deserialize<Dictionary<string, object>>(json)
                    ?? new Dictionary<string, object>();
            }
        }
        catch
        {
            _settings = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Sozlamalarni saqlash
    /// </summary>
    private void SaveSettings()
    {
        try
        {
            var directory = Path.GetDirectoryName(SettingsFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(SettingsFilePath, json);
        }
        catch
        {
            // Log error
        }
    }
}
