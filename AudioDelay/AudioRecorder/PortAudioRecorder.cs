using System.Runtime.InteropServices;
using AudioDelay.Args;
using PortAudioSharp;
using PortAudioStream = PortAudioSharp.Stream;

namespace AudioDelay.AudioRecorder;

public class PortAudioRecorder : AudioRecorder
{
  private const double SampleRate = 44100;
  private const int ChannelCount = 1;
  private const uint FramesPerBuffer = 2048;
  private const int BytesPerSample = sizeof(short);

  private readonly PortAudioStream _inputStream;
  private readonly PortAudioStream _outputStream;
  private readonly Queue<byte> _audioQueue = new();
  private readonly object _queueLock = new();
  private readonly bool _portAudioInitialized;
  private bool _disposed;

  public PortAudioRecorder(Arguments args) : base(args)
  {
    try
    {
      PortAudio.Initialize();
      _portAudioInitialized = true;
    }
    catch (PortAudioException ex) when (OperatingSystem.IsLinux())
    {
      throw new InvalidOperationException(
        "PortAudio could not initialize on Linux. This is usually caused by the system audio stack or PortAudio host probing. Install libasound2-dev or portaudio19-dev, make sure a default input and output device exist, and if JACK is not in use prefer running with PulseAudio/PipeWire available.",
        ex);
    }

    var inputDevice = ResolveInputDevice(args.InputDevice);
    var outputDevice = ResolveOutputDevice(args.OutputDevice);

    var inputInfo = PortAudio.GetDeviceInfo(inputDevice);
    var outputInfo = PortAudio.GetDeviceInfo(outputDevice);

    _inputStream = new PortAudioStream(
      CreateInputParameters(inputDevice, inputInfo),
      null,
      SampleRate,
      FramesPerBuffer,
      StreamFlags.NoFlag,
      InputCallback,
      null);

    _outputStream = new PortAudioStream(
      null,
      CreateOutputParameters(outputDevice, outputInfo),
      SampleRate,
      FramesPerBuffer,
      StreamFlags.NoFlag,
      OutputCallback,
      null);
  }

  public override void Play()
  {
    _outputStream.Start();
  }

  public override void StopPlayback()
  {
    if (!_outputStream.IsStopped)
      _outputStream.Stop();
  }

  public override void Start()
  {
    _inputStream.Start();
  }

  public override void Stop()
  {
    if (!_inputStream.IsStopped)
      _inputStream.Stop();
  }

  public override void Dispose()
  {
    if (_disposed)
      return;

    _disposed = true;

    _inputStream.Dispose();
    _outputStream.Dispose();
    if (_portAudioInitialized)
      PortAudio.Terminate();
  }

  private static StreamParameters CreateInputParameters(int device, DeviceInfo deviceInfo) =>
    new()
    {
      device = device,
      channelCount = ChannelCount,
      sampleFormat = SampleFormat.Int16,
      suggestedLatency = deviceInfo.defaultLowInputLatency,
      hostApiSpecificStreamInfo = IntPtr.Zero
    };

  private static StreamParameters CreateOutputParameters(int device, DeviceInfo deviceInfo) =>
    new()
    {
      device = device,
      channelCount = ChannelCount,
      sampleFormat = SampleFormat.Int16,
      suggestedLatency = deviceInfo.defaultLowOutputLatency,
      hostApiSpecificStreamInfo = IntPtr.Zero
    };

  private static int ResolveInputDevice(int requestedDevice)
  {
    if (requestedDevice >= 0)
      return requestedDevice;

    var defaultDevice = PortAudio.DefaultInputDevice;
    if (defaultDevice == PortAudio.NoDevice)
      throw new InvalidOperationException("No default input device is available.");

    return defaultDevice;
  }

  private static int ResolveOutputDevice(int requestedDevice)
  {
    if (requestedDevice >= 0)
      return requestedDevice;

    var defaultDevice = PortAudio.DefaultOutputDevice;
    if (defaultDevice == PortAudio.NoDevice)
      throw new InvalidOperationException("No default output device is available.");

    return defaultDevice;
  }

  private StreamCallbackResult InputCallback(
    IntPtr input,
    IntPtr output,
    uint frameCount,
    ref StreamCallbackTimeInfo timeInfo,
    StreamCallbackFlags statusFlags,
    IntPtr userDataPtr)
  {
    if (input == IntPtr.Zero)
      return StreamCallbackResult.Continue;

    var byteCount = checked((int)frameCount * ChannelCount * BytesPerSample);
    var buffer = new byte[byteCount];
    Marshal.Copy(input, buffer, 0, byteCount);

    lock (_queueLock)
    {
      foreach (var sample in buffer)
        _audioQueue.Enqueue(sample);
    }

    return StreamCallbackResult.Continue;
  }

  private StreamCallbackResult OutputCallback(
    IntPtr input,
    IntPtr output,
    uint frameCount,
    ref StreamCallbackTimeInfo timeInfo,
    StreamCallbackFlags statusFlags,
    IntPtr userDataPtr)
  {
    var byteCount = checked((int)frameCount * ChannelCount * BytesPerSample);
    var buffer = new byte[byteCount];

    lock (_queueLock)
    {
      var bytesToCopy = Math.Min(byteCount, _audioQueue.Count);
      for (var i = 0; i < bytesToCopy; i++)
        buffer[i] = _audioQueue.Dequeue();
    }

    Marshal.Copy(buffer, 0, output, byteCount);
    return StreamCallbackResult.Continue;
  }
}
