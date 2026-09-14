using System.Runtime.InteropServices;
using AudioDelay.Args;
using AudioDelay.Interop;
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
    var initError = PortAudioNative.Initialize();
    if (initError != 0)
      throw CreateInitializationException(initError);
    _portAudioInitialized = true;

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
      PortAudioNative.Terminate();
  }

  private static Exception CreateInitializationException(int errorCode)
  {
    var message = PortAudioNative.GetErrorMessage(errorCode);

    if (OperatingSystem.IsLinux())
    {
      return new InvalidOperationException(
        $"PortAudio could not initialize on Linux: {message}. This usually means PortAudio host probing hit a broken ALSA/JACK configuration. Ensure the machine has working default capture and playback devices; if JACK is not used, prefer a clean ALSA/PipeWire/PulseAudio default-device setup.");
    }

    return new InvalidOperationException($"PortAudio could not initialize: {message}");
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
