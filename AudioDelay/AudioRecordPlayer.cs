using NAudio.Wave;

namespace AudioDelay;
public class AudioRecorderPlayer
{
    private readonly WaveInEvent _waveIn;
    private readonly WaveOutEvent _waveOut;
    private readonly BufferedWaveProvider _bufferedWaveProvider;

    public AudioRecorderPlayer(int recordingLength)
    {
        // Set up recording
        _waveIn = new WaveInEvent();
        _waveIn.DataAvailable += OnDataAvailable!;
        _waveIn.WaveFormat = new WaveFormat(44100, 1); // CD quality audio, mono channel

        // Set up playback
        _waveOut = new WaveOutEvent();
        _bufferedWaveProvider = new BufferedWaveProvider(_waveIn.WaveFormat)
        {
            BufferDuration = TimeSpan.FromMilliseconds(recordingLength)
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