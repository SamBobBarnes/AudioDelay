using AudioDelay.Args;
using PortAudioSharp;

namespace AudioDelay.AudioRecorder;

public class PortAudioRecorder : AudioRecorder
{
  public PortAudioRecorder(Arguments args) : base(args)
  {
    // Initialize PortAudio and set up the audio stream here
    // This is a placeholder for the actual implementation
    PortAudio.Initialize();
    var inputParams = new StreamParameters
    {
      device = PortAudio.DefaultInputDevice,
      channelCount = 1, // Mono
      sampleFormat = SampleFormat.Float32,
      suggestedLatency = PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultLowInputLatency,
      hostApiSpecificStreamInfo = IntPtr.Zero
    };
    var putputParams = new StreamParameters
    {
      device = PortAudio.DefaultOutputDevice,
      channelCount = 1, // Mono
      sampleFormat = SampleFormat.Float32,
      suggestedLatency = PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultLowInputLatency,
      hostApiSpecificStreamInfo = IntPtr.Zero
    };

    // PortAudioSharp.Stream stream = new PortAudioSharp.Stream(inParams: inputParams, outParams: null, sampleRate: options.SampleRate,
    //   framesPerBuffer: 0,
    //   streamFlags: StreamFlags.ClipOff,
    //   callback: callback,
    //   userData: IntPtr.Zero
    // );
  }

  public override void Play()
  {
    throw new NotImplementedException();
  }

  public override void StopPlayback()
  {
    throw new NotImplementedException();
  }

  public override void Start()
  {
    throw new NotImplementedException();
  }

  public override void Stop()
  {
    throw new NotImplementedException();
  }
}