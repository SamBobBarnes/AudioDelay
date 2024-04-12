using AudioDelay.Args;
using Alsa.Net;

namespace AudioDelay.AudioRecorder;

public class LinuxAudioRecorder: AudioRecorder
{
    private readonly MemoryStream _buffer;
    private readonly ISoundDevice _soundDevice;
    private readonly CancellationTokenSource _token;
    public LinuxAudioRecorder(Arguments args) : base(args)
    {
        var soundDeviceSettings = new SoundDeviceSettings
        {
            RecordingSampleRate = 44100,
            RecordingChannels = 1
        };
        _soundDevice = AlsaDeviceBuilder.Create(soundDeviceSettings);
        
        var streamSize = (int)(soundDeviceSettings.RecordingSampleRate * soundDeviceSettings.RecordingChannels * 2 * args.RecordingLength/1000);
        _buffer = new MemoryStream(streamSize);
        _token = new CancellationTokenSource(TimeSpan.FromMilliseconds(args.RecordingLength));
    }

    public override void Play()
    {
        _soundDevice.Play(_buffer);
    }

    public override void StopPlayback()
    {
        Console.WriteLine("Stop playback not supported on Linux");
    }

    public override void Start()
    {
        _soundDevice.Record(_buffer, _token.Token);
    }

    public override void Stop()
    {
        Console.WriteLine("Stop recording not supported on Linux");
    }

    public override void Dispose()
    {
        _soundDevice.Dispose();
        _token.Dispose();
        _buffer.Dispose();
    }
}