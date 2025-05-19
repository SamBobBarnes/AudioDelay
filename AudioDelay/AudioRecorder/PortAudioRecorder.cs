using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using AudioDelay.Args;
using PortAudioSharp;
using Stream = PortAudioSharp.Stream;

namespace AudioDelay.AudioRecorder;

public class PortAudioRecorder : AudioRecorder
{
  private readonly Stream _inputStream;
  private MemoryStream _memoryStream;
  private readonly Stream _outputStream;

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
    var outputParams = new StreamParameters
    {
      device = PortAudio.DefaultOutputDevice,
      channelCount = 1, // Mono
      sampleFormat = SampleFormat.Float32,
      suggestedLatency = PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultLowInputLatency,
      hostApiSpecificStreamInfo = IntPtr.Zero
    };

    var sampleRate = (int)PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultSampleRate;
    uint framesPerBuffer = 256;
    var recordedSamples = new float[sampleRate * 3];
    var sampleIndex = 0;
    var totalFrames = recordedSamples.Length;

    Stream.Callback callback = (
      IntPtr inputBuffer,
      IntPtr outputBuffer,
      uint frameCount,
      ref StreamCallbackTimeInfo timeInfo,
      StreamCallbackFlags statusFlags,
      IntPtr userDataPtr
    ) =>
    {
      var framesToCopy = (int)Math.Min(frameCount, totalFrames - sampleIndex);
      if (framesToCopy > 0)
      {
        var buffer = new float[framesToCopy];
        Marshal.Copy(inputBuffer, buffer, 0, framesToCopy);
        Array.Copy(buffer, 0, recordedSamples, sampleIndex, framesToCopy);
        sampleIndex += framesToCopy;
      }

      return StreamCallbackResult.Continue;
    };

    _inputStream = new Stream(inputParams,
      null,
      PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultSampleRate,
      0,
      StreamFlags.NoFlag,
      callback,
      null
    );

    var playFinished = false;
    float[]? lastSampleArray = null;
    var lastIndex = 0; // not played
    var dataItems = new BlockingCollection<float[]>(recordedSamples.Length);
    dataItems.Add(recordedSamples);
    dataItems.CompleteAdding();

    Stream.Callback playCallback = (
      IntPtr input,
      IntPtr output,
      uint frameCount,
      ref StreamCallbackTimeInfo timeInfo,
      StreamCallbackFlags statusFlags,
      IntPtr userData
    ) =>
    {
      if (dataItems.IsCompleted && lastSampleArray == null && lastIndex == 0)
      {
        Console.WriteLine("Finished playing");
        playFinished = true;
        return StreamCallbackResult.Complete;
      }

      var expected = Convert.ToInt32(frameCount);
      var i = 0;

      while ((lastSampleArray != null || dataItems.Count != 0) && i < expected)
      {
        var needed = expected - i;

        if (lastSampleArray != null)
        {
          var remaining = lastSampleArray.Length - lastIndex;
          if (remaining >= needed)
          {
            var this_block = lastSampleArray.Skip(lastIndex).Take(needed).ToArray();
            lastIndex += needed;
            if (lastIndex == lastSampleArray.Length)
            {
              lastSampleArray = null;
              lastIndex = 0;
            }

            Marshal.Copy(this_block, 0, IntPtr.Add(output, i * sizeof(float)), needed);
            return StreamCallbackResult.Continue;
          }

          var this_block2 = lastSampleArray.Skip(lastIndex).Take(remaining).ToArray();
          lastIndex = 0;
          lastSampleArray = null;

          Marshal.Copy(this_block2, 0, IntPtr.Add(output, i * sizeof(float)), remaining);
          i += remaining;
          continue;
        }

        if (dataItems.Count != 0)
        {
          lastSampleArray = dataItems.Take();
          lastIndex = 0;
        }
      }

      if (i < expected)
      {
        var sizeInBytes = (expected - i) * 4;
        Marshal.Copy(new byte[sizeInBytes], 0, IntPtr.Add(output, i * sizeof(float)), sizeInBytes);
      }

      return StreamCallbackResult.Continue;
    };

    _outputStream = new Stream(null,
      outputParams,
      PortAudio.GetDeviceInfo(PortAudio.DefaultOutputDevice).defaultSampleRate,
      0,
      StreamFlags.NoFlag,
      playCallback,
      null
    );
  }

  public override void Play()
  {
    _outputStream.Start();
  }

  public override void StopPlayback()
  {
    _outputStream.Stop();
    _outputStream.Close();
  }

  public override void Start()
  {
    // Start the audio stream
    _inputStream.Start();
  }

  public override void Stop()
  {
    // Stop the audio stream
    _inputStream.Stop();
    _inputStream.Close();
  }
}