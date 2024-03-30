using NAudio.Wave;

namespace AudioDelay;
public class AudioRecorderPlayer
{
    private WaveInEvent waveIn;
    private WaveOutEvent waveOut;
    private BufferedWaveProvider bufferedWaveProvider;

    public AudioRecorderPlayer()
    {
        // Set up recording
        waveIn = new WaveInEvent();
        waveIn.DataAvailable += OnDataAvailable;
        waveIn.WaveFormat = new WaveFormat(44100, 1); // CD quality audio, mono channel

        // Set up playback
        waveOut = new WaveOutEvent();
        bufferedWaveProvider = new BufferedWaveProvider(waveIn.WaveFormat);
        bufferedWaveProvider.BufferDuration = TimeSpan.FromMinutes(10);
        waveOut.Init(bufferedWaveProvider);
    }

    public void Play()
    {
        waveOut.Play();
    }

    public void StopPlayback()
    {
        waveOut.Stop();
    }
    
    public void Start()
    {
        waveIn.StartRecording();
        // waveOut.Play();
    }

    public void Stop()
    {
        waveIn.StopRecording();
        // waveOut.Stop();
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
    }
}