using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using AppServices = Metra.Application.Services.Interfaces;
using Metra.Application.Configuration;
using Metra.Application.Services.Implementation;
using Metra.Application.Services.Service;
using Metra.Infrastructure.Logging;
using Metra.Infrastructure.Persistence.Settings;
using Metra.Infrastructure.Services;
using Notification.Wpf;
using Syncfusion.Licensing;

namespace Metra.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static MainWindow? GlobalMainWindow { get; set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Syncfusion license (TODO: Add your license key)
         SyncfusionLicenseProvider.RegisterLicense("MzUzODIyNUAzMjM3MmUzMDJlMzBMSUZBbU53aEdqelU3bWZjMXNoOVdoakI1aTNML0Fuek9WMVc5YloyNUtBPQ==");

        // Serilog setup
        Log.Logger = LoggingConfiguration.ConfigureLogging();

        try
        {
            Log.Information("Metra ilovasi ishga tushmoqda...");

            // DI Container
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);

            // LoginWindow ni ko'rsatish
            var loginWindow = ServiceProvider.GetRequiredService<Views.LoginWindow>();
            loginWindow.Show();

            Log.Information("Metra ilovasi muvaffaqiyatli ishga tushdi");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ilova ishga tushayotganda xatolik yuz berdi");
            MessageBox.Show(
                $"Ilova ishga tushayotganda xatolik yuz berdi:\n{ex.Message}",
                "Xatolik",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddSerilog();
        });

        // Notification Manager - Singleton
        services.AddSingleton<NotificationManager>();

        // Settings - Singleton
        services.AddSingleton<AppSettings>();

        // Application Services - Singleton
        services.AddSingleton<AppServices.ITokenService, TokenService>();

        // AuthService with HttpClient factory
        services.AddHttpClient<AppServices.IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
        });

        // FakturaService with HttpClient factory
        services.AddHttpClient<AppServices.IFakturaService, FakturaService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
        });

        // FilialService with HttpClient factory
        services.AddHttpClient<AppServices.IFilialService, FilialService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);

            // HTTP headers (eski versiya bilan mos kelishi uchun)
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("en-US"));
            client.DefaultRequestHeaders.AcceptLanguage.Add(
                new System.Net.Http.Headers.StringWithQualityHeaderValue("en", 0.9));
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
                                   | System.Net.DecompressionMethods.Deflate
        });

        // MijozService with HttpClient factory
        services.AddHttpClient<AppServices.IMijozService, MijozService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
        });

        // Desktop Services yo'q - oddiy DI yetarli

        // HttpClient for general API calls
        services.AddHttpClient("MetraApi", client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
        });

        // ViewModels
        services.AddTransient<ViewModels.LoginViewModel>();
        services.AddTransient<ViewModels.FilialViewModel>();
        services.AddTransient<ViewModels.MijozListViewModel>();
        services.AddTransient<ViewModels.MijozFormViewModel>();

        // Views/Windows - Transient
        services.AddTransient<Views.LoginWindow>();
        services.AddTransient<MainWindow>();

        // Pages - Transient (soddalashtirilgan)
        services.AddTransient<Views.Pages.MijozListPage>();
        services.AddTransient<Views.Pages.FilialPage>();

        // Dialog Windows - Transient (soddalashtirilgan)
        services.AddTransient<Views.Windows.MijozFormWindow>();
        services.AddTransient<Views.Windows.FilialAddEditWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Metra ilovasi yopilmoqda...");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}

