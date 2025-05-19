using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using AudioDelay.Args;
using PortAudioSharp;
using Stream = PortAudioSharp.Stream;

namespace AudioDelay.AudioRecorder;

public class PortAudioRecorder : AudioRecorder
{
  private readonly Stream _inputStream;
  private readonly Stream _outputStream;

  public PortAudioRecorder(Arguments args) : base(args)
  {
    PortAudio.Initialize();

    #region Input

    var inputParams = new StreamParameters //TODO: allow for non default devices 
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
    var recordedSamples = new float[sampleRate * 3];
    var sampleIndex = 0;
    var totalFrames = recordedSamples.Length;

    StreamCallbackResult Callback(
      IntPtr inputBuffer,
      IntPtr outputBuffer,
      uint frameCount,
      ref StreamCallbackTimeInfo timeInfo,
      StreamCallbackFlags statusFlags,
      IntPtr userDataPtr
    )
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
    }

    _inputStream = new Stream(inputParams,
      null,
      PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).defaultSampleRate,
      0,
      StreamFlags.NoFlag,
      Callback,
      null
    );

    #endregion

    #region Output

    float[]? lastSampleArray = null;
    var lastIndex = 0;
    var dataItems = new BlockingCollection<float[]>(recordedSamples.Length);
    dataItems.Add(recordedSamples);
    dataItems.CompleteAdding();

    StreamCallbackResult PlayCallback(
      IntPtr input,
      IntPtr output,
      uint frameCount,
      ref StreamCallbackTimeInfo timeInfo,
      StreamCallbackFlags statusFlags,
      IntPtr userData
    )
    {
      if (dataItems.IsCompleted && lastSampleArray == null && lastIndex == 0)
      {
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
            var thisBlock = lastSampleArray.Skip(lastIndex).Take(needed).ToArray();
            lastIndex += needed;
            if (lastIndex == lastSampleArray.Length)
            {
              lastSampleArray = null;
              lastIndex = 0;
            }

            Marshal.Copy(thisBlock, 0, IntPtr.Add(output, i * sizeof(float)), needed);
            return StreamCallbackResult.Continue;
          }

          var thisBlock2 = lastSampleArray.Skip(lastIndex).Take(remaining).ToArray();
          lastIndex = 0;
          lastSampleArray = null;

          Marshal.Copy(thisBlock2, 0, IntPtr.Add(output, i * sizeof(float)), remaining);
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
    }

    _outputStream = new Stream(null,
      outputParams,
      PortAudio.GetDeviceInfo(PortAudio.DefaultOutputDevice).defaultSampleRate,
      0,
      StreamFlags.NoFlag,
      PlayCallback,
      null
    );

    #endregion
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
    _inputStream.Start();
  }

  public override void Stop()
  {
    _inputStream.Stop();
    _inputStream.Close();
  }
}