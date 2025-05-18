using NAudio.Wave;
using NotoNote.Models;
using System.Diagnostics;
using System.IO;

namespace NotoNote.Services;
public sealed class AudioService : IAudioService
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _waveFileWriter;
    private const string FileName = "input.wav";
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);

    private Task? _timeoutTimer;
    private CancellationTokenSource? _timeoutCts;

    // Whisper API制限（25MB）を超えない時間
    private readonly TimeSpan DefaultTimeout = TimeSpan.FromMinutes(10);

    public void StartRecording(Action? timeoutCallback)
    {
        if (_waveIn != null) throw new InvalidOperationException("Already recording");

        _waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(16000, 16, 1) // whisper推奨
        };

        _waveFileWriter = new WaveFileWriter(FilePath, _waveIn.WaveFormat);

        _waveIn.DataAvailable += (_, e) => _waveFileWriter!.Write(e.Buffer, 0, e.BytesRecorded);
        _waveIn.StartRecording();

        StartTimeoutTimer(timeoutCallback);
    }

    public Task<WaveFilePath> StopRecordingAsync()
    {
        StopTimeoutTimer();
        if (_waveIn == null)
        {
            Debug.WriteLine("WaveIn is null, cannot stop recording.");
            return Task.FromResult(new WaveFilePath(FilePath));
        }

        var tcs = new TaskCompletionSource<WaveFilePath>();

        _waveIn.RecordingStopped += (_, _) =>
        {
            _waveFileWriter!.Dispose();
            _waveFileWriter = null;

            _waveIn?.Dispose();
            _waveIn = null;

            tcs.SetResult(new(FilePath));
        };

        _waveIn.StopRecording();
        return tcs.Task;
    }

    public void Dispose()
    {
        _waveIn?.Dispose();
        _waveFileWriter?.Dispose();
    }

    public void StartTimeoutTimer(Action? timeoutCallback)
    {
        StopTimeoutTimer();

        _timeoutCts = new CancellationTokenSource();
        var ct = _timeoutCts.Token;
        _timeoutTimer = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(DefaultTimeout, _timeoutCts.Token);
                if (!ct.IsCancellationRequested)
                {
                    Debug.WriteLine("Timeout occurred, stopping recording.");
                    await StopRecordingAsync();

                    timeoutCallback?.Invoke();
                }
            }
            catch (TaskCanceledException)
            {
            }
        }, ct);
    }

    public void StopTimeoutTimer()
    {
        _timeoutCts?.Cancel();
        _timeoutCts?.Dispose();
        _timeoutCts = null;
        _timeoutTimer = null;
    }
}
