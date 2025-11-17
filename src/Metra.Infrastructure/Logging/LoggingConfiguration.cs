using Serilog;
using Serilog.Events;

namespace Metra.Infrastructure.Logging;

/// <summary>
/// Logging konfiguratsiyasi
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Serilog ni sozlash
    /// </summary>
    public static ILogger ConfigureLogging()
    {
        var logsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Metra",
            "logs",
            "metra-.txt");

        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                logsPath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
