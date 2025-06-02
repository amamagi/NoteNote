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

        // UIスレッドの未処理例外で発生
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        // UIスレッド以外の未処理例外で発生
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        // それでも処理されない例外で発生
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

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


    private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        var exception = e.Exception;
        HandleException(exception);
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        var exception = e.Exception.InnerException as Exception;
        HandleException(exception);
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        HandleException(exception);
    }

    private void HandleException(Exception? e)
    {
        System.Windows.MessageBox.Show($"申し訳ありません。エラーが発生したためアプリケーションを終了します。\n\n--エラー内容--\n{e?.ToString()}");
        Environment.Exit(1);
    }
}

