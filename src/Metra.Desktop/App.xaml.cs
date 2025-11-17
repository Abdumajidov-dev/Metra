using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using AppServices = Metra.Application.Services.Interfaces;
using Metra.Infrastructure.API;
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

    protected override void OnStartup(StartupEventArgs e)
    {
        // Syncfusion license (TODO: Add your license key)
        // SyncfusionLicenseProvider.RegisterLicense("YOUR_LICENSE_KEY");

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

            // MainWindow ni ko'rsatish
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

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

        // TODO: Add more services
        // services.AddSingleton<INavigationService, NavigationService>();
        // services.AddSingleton<IDialogService, DialogService>();
        // services.AddSingleton<IAuthService, AuthService>();

        // HttpClient for API calls
        services.AddHttpClient("MetraApi", client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ApiConfig.TimeoutSeconds);
        });

        // ViewModels - Transient
        // TODO: Add ViewModels
        // services.AddTransient<MainWindowViewModel>();
        // services.AddTransient<LoginViewModel>();

        // Views/Windows - Transient
        services.AddTransient<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Metra ilovasi yopilmoqda...");
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}

