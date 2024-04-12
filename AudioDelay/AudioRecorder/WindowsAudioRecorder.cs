using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace AudioDelay.AudioRecorder;
public class WindowsAudioRecorder : IAudioRecorder
{
    private readonly WaveInEvent _waveIn;
    private readonly WaveOutEvent _waveOut;
    private readonly BufferedWaveProvider _bufferedWaveProvider;

    public WindowsAudioRecorder(int recordingLength)
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

    public void GetDevices()
    {
        var enumerator = new MMDeviceEnumerator();
        foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
        {
            Console.WriteLine("{0} ({1})", endpoint.FriendlyName, endpoint.ID);
        }
    }

    private void OnDataAvailable(object sender, WaveInEventArgs e)
    {
        _bufferedWaveProvider.AddSamples(e.Buffer, 0, e.BytesRecorded);
    }
}