using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotoNote.DataStore;
using NotoNote.Models;
using NotoNote.Services;
using NotoNote.ViewModels;
using NotoNote.Views;
using System.Diagnostics;
using System.Windows;
using Application = System.Windows.Application;

namespace NotoNote;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.Configure<List<OpenAiCompatibleApiOptions>>(ctx.Configuration.GetSection("OpenAiCompatibleApi"));

                // DataStore
                services.AddSingleton<IPresetProfileProvider, PresetProfileProvider>();
                services.AddSingleton<ILiteDbContext, LiteDbContext>();
                services.AddSingleton<IApiKeyRepository, ApiKeyRepository>();
                services.AddSingleton<IProfileRepository, ProfileRepository>();
                services.AddSingleton<IHotkeyRepository, HotkeyRepository>();

                // Services
                services.AddSingleton<ModelCollector>();
                services.AddSingleton<ITranscriptionModelProvider>(sp => sp.GetRequiredService<ModelCollector>());
                services.AddSingleton<IChatModelProvider>(sp => sp.GetRequiredService<ModelCollector>());
                services.AddSingleton<IApiMetadataProvider>(sp => sp.GetRequiredService<ModelCollector>());
                services.AddSingleton<IWindowService, WindowService>();
                services.AddSingleton<IHotkeyService, HotkeyService>((_) => new HotkeyService(Current.Dispatcher));
                services.AddSingleton<IClipBoardService, ClipBoardService>((_) => new ClipBoardService(Current.Dispatcher));
                services.AddSingleton<IAudioService, AudioService>();
                services.AddSingleton<ITranscriptionServiceFactory, TranscriptionServiceFactory>();
                services.AddSingleton<IChatServiceFactory, ChatServiceFactory>();

                // Views/ViewModels
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<SettingsView>();
                services.AddSingleton<MainScreenViewModel>();
                services.AddSingleton<MainScreenView>();
                services.AddSingleton<MainWindow>();
            })
            .ConfigureLogging(b =>
            {
                b.AddDebug();
            })
            .Build();
        Debug.WriteLine(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT"));

        var main = _host.Services.GetRequiredService<MainWindow>();
        main.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null) await _host.StopAsync();
        base.OnExit(e);
    }
}

