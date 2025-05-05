using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotoNote.Services;
using NotoNote.ViewModels;
using System.Configuration;
using System.Data;
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
                services.Configure<OpenAiOptions>(
                    ctx.Configuration.GetSection("OpenAI"));
                Debug.WriteLine(ctx.Configuration.GetSection("OpenAI"));
                services.AddSingleton<IAudioService, AudioService>();
                services.AddSingleton<ITranscriptionService, OpenATranscription>();
                services.AddSingleton<ILanguageProcessingService, OpenAiChat>();

                services.AddSingleton<MainViewModel>();
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

