using AudioDelay.Args;
using NAudio.Wave;

namespace AudioDelay.AudioRecorder;
public class WindowsAudioRecorder : AudioRecorder
{
    private readonly WaveInEvent _waveIn;
    private readonly WaveOutEvent _waveOut;
    private readonly BufferedWaveProvider _bufferedWaveProvider;

    public WindowsAudioRecorder(Arguments args): base(args)
    {
        // Set up recording
        _waveIn = new WaveInEvent();
        _waveIn.DataAvailable += OnDataAvailable!;
        _waveIn.WaveFormat = new WaveFormat(44100, 1); // CD quality audio, mono channel

        // Set up playback
        _waveOut = new WaveOutEvent();
        _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat)
        {
            BufferDuration = TimeSpan.FromMilliseconds(args.RecordingLength),
        };
        _waveOut.Init(_bufferedWaveProvider);
    }

    public override void Play()
    {
        _waveOut.Play();
    }

    public override void StopPlayback()
    {
        _waveOut.Stop();
    }
    
    public override void Start()
    {
        _waveIn.StartRecording();
    }

    public override void Stop()
    {
        _waveIn.StopRecording();
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
    }
}