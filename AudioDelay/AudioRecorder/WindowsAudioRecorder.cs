using AudioDelay.Args;
using NAudio.Wave;

namespace AudioDelay.AudioRecorder;
public class WindowsAudioRecorder : IAudioRecorder
{
    private readonly WaveInEvent _waveIn;
    private readonly WaveOutEvent _waveOut;
    private readonly BufferedWaveProvider _bufferedWaveProvider;

    public WindowsAudioRecorder(Arguments args)
    {
        // Set up recording
        _waveIn = new WaveInEvent();
        _waveIn.DeviceNumber = args.InputDevice-1;
        _waveIn.DataAvailable += OnDataAvailable!;
        _waveIn.WaveFormat = new WaveFormat(44100, 1); // CD quality audio, mono channel

        // Set up playback
        _waveOut = new WaveOutEvent();
        _waveOut.DeviceNumber = args.OutputDevice;
        _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat)
        {
            BufferDuration = TimeSpan.FromMilliseconds(args.RecordingLength),
        };
        _waveOut.Init(_bufferedWaveProvider);
    }

    public void Play()
    {
        _waveOut.Play();
    }

    public void StopPlayback()
    {
        _waveOut.Stop();
    }
    
    public void Start()
    {
        _waveIn.StartRecording();
    }

    public void Stop()
    {
        _waveIn.StopRecording();
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
    }
}