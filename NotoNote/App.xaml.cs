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
                services.Configure<OpenAiOptions>(ctx.Configuration.GetSection("OpenAI"));
                services.Configure<List<PresetProfileOptions>>(ctx.Configuration.GetSection("PresetProfiles"));

                services.AddSingleton<IWindowService, WindowService>();
                services.AddSingleton<ILiteDbContext, LiteDbContext>();
                services.AddSingleton<IPresetProfileProvider, PresetProfileProvider>();
                services.AddSingleton<IProfileRepository, ProfileRepository>();
                services.AddSingleton<IHotkeyService, HotkeyService>((_) => new HotkeyService(Current.Dispatcher));
                services.AddSingleton<IClipBoardService, ClipBoardService>((_) => new ClipBoardService(Current.Dispatcher));
                services.AddSingleton<IApiKeyRegistry, OptionApiKeyRegistry>();
                services.AddSingleton<IAudioService, AudioService>();
                services.AddSingleton<ITranscriptionAiServiceFactory, TranscriptionAiServiceFactory>();
                services.AddSingleton<IChatAiServiceFactory, ChatAiServiceFactory>();
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

