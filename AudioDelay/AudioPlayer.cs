using NAudio.Wave;

namespace AudioDelay;
public class AudioPlayer
{
    private WaveOutEvent waveOut;
    private BufferedWaveProvider bufferedWaveProvider;

    public AudioPlayer(byte[] audioBuffer, WaveFormat waveFormat)
    {
        waveOut = new WaveOutEvent();
        waveOut.DeviceNumber = -1;
        bufferedWaveProvider = new BufferedWaveProvider(waveFormat)
        {
            BufferLength = audioBuffer.Length,
            DiscardOnBufferOverflow = true
        };
        bufferedWaveProvider.AddSamples(audioBuffer, 0, audioBuffer.Length);
        waveOut.Init(bufferedWaveProvider);
    }

    public void Play()
    {
        waveOut.Play();
    }

    public void Stop()
    {
        waveOut.Stop();
    }

    public void Dispose()
    {
        waveOut.Dispose();
    }
}
