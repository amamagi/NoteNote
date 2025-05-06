using NAudio.Wave;
using NotoNote.Models;
using System.IO;

namespace NotoNote.Services;
public sealed class AudioService : IAudioService
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _waveFileWriter;
    private const string FileName = "input.wav";
    private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);

    public void StartRecording()
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
    }

    public Task<WaveFilePath> StopRecordingAsync()
    {
        if (_waveIn == null) throw new InvalidOperationException("Not recording");

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
}
