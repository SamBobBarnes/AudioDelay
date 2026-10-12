using AudioDelay.Args;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioDelay.AudioRecorder;
public class WindowsAudioRecorder : AudioRecorder
{
    private static readonly (float Frequency, int DurationMs)[] TestTonePattern =
    [
        (440f, 250),
        (660f, 250),
        (880f, 400)
    ];

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

    public override void PlayTestOutput()
    {
        using var waveOut = new WaveOutEvent();

        foreach (var (frequency, durationMs) in TestTonePattern)
        {
            var signalGenerator = new SignalGenerator()
            {
                Gain = 0.2,
                Frequency = frequency,
                Type = SignalGeneratorType.Sin
            };
            signalGenerator.Frequency = frequency;
            var tone = new OffsetSampleProvider(signalGenerator)
            {
                TakeSamples = signalGenerator.WaveFormat.SampleRate * durationMs / 1000
            };

            waveOut.Stop();
            waveOut.Init(tone);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
              Thread.Sleep(10);
        }
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

    public override void Shutdown()
    {
        Stop();
        StopPlayback();
    }

    public override void Dispose()
    {
        _waveIn.Dispose();
        _waveOut.Dispose();
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
    }
}