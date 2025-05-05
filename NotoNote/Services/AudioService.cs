using NAudio.Wave;
using System.IO;

namespace NotoNote.Services;
public sealed class AudioService : IAudioService
{
    private WaveInEvent? _waveIn;
    private WaveFileWriter? _waveFileWriter;
    private MemoryStream? _memoryStream;

    public void StartRecording()
    {
        if (_waveIn != null) throw new InvalidOperationException("Already recording");

        _waveIn = new WaveInEvent
        {
            DeviceNumber = 0,
            WaveFormat = new WaveFormat(16000, 16, 1) // whisper推奨
        };

        _memoryStream = new MemoryStream();
        _waveFileWriter = new WaveFileWriter(_memoryStream, _waveIn.WaveFormat);

        _waveIn.DataAvailable += (_, e) => _waveFileWriter!.Write(e.Buffer, 0, e.BytesRecorded);
        _waveIn.StartRecording();
    }

    public Task<byte[]> StopRecordingAsync()
    {
        if (_waveIn == null) throw new InvalidOperationException("Not recording");

        var tcs = new TaskCompletionSource<byte[]>();

        _waveIn.RecordingStopped += (_, _) =>
        {
            var bytes = _memoryStream!.ToArray();
            _waveFileWriter!.Dispose();
            _waveFileWriter = null;
            _memoryStream!.Dispose();
            _memoryStream = null;

            _waveIn?.Dispose();
            _waveIn = null;

            tcs.SetResult(bytes);
        };

        _waveIn.StopRecording();
        return tcs.Task;
    }
    public void Dispose()
    {
        _waveIn?.Dispose();
        _waveFileWriter?.Dispose();
        _memoryStream?.Dispose();
    }
}
