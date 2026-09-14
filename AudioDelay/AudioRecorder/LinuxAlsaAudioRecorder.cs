using AudioDelay.Args;
using AudioDelay.Interop;

namespace AudioDelay.AudioRecorder;

public class LinuxAlsaAudioRecorder : AudioRecorder
{
  private const uint SampleRate = 48000;
  private const uint ChannelCount = 1;
  private const int BytesPerSample = sizeof(short);
  private const int PeriodFrames = 1024;
  private const uint LatencyUs = 100000;

  private readonly int _delayBytes;
  private readonly IntPtr _captureHandle;
  private readonly IntPtr _playbackHandle;
  private readonly List<byte> _audioBuffer = [];
  private readonly object _bufferLock = new();
  private readonly object _stateLock = new();
  private readonly byte[] _captureBuffer = new byte[PeriodFrames * ChannelCount * BytesPerSample];
  private readonly byte[] _playbackBuffer = new byte[PeriodFrames * ChannelCount * BytesPerSample];
  private readonly byte[] _silenceBuffer = new byte[PeriodFrames * ChannelCount * BytesPerSample];
  private Thread? _captureThread;
  private Thread? _playbackThread;
  private int _playbackReadOffset;
  private bool _captureRunning;
  private bool _playbackRunning;
  private bool _disposed;

  public LinuxAlsaAudioRecorder(Arguments args) : base(args)
  {
    _delayBytes = checked((int)((long)args.Delay * SampleRate * ChannelCount * BytesPerSample / 1000));
    _captureHandle = AlsaNative.OpenPcm("default", AlsaNative.CaptureStream);
    _playbackHandle = AlsaNative.OpenPcm("default", AlsaNative.PlaybackStream);

    try
    {
      AlsaNative.ConfigurePcm(_captureHandle, ChannelCount, SampleRate, LatencyUs);
      AlsaNative.ConfigurePcm(_playbackHandle, ChannelCount, SampleRate, LatencyUs);
    }
    catch
    {
      AlsaNative.Close(_captureHandle);
      AlsaNative.Close(_playbackHandle);
      throw;
    }
  }

  public override void Play()
  {
    lock (_stateLock)
    {
      if (_playbackRunning)
        return;

      _playbackRunning = true;
      _playbackThread = new Thread(PlaybackLoop)
      {
        IsBackground = true,
        Name = "alsa-playback"
      };
      _playbackThread.Start();
    }
  }

  public override void StopPlayback()
  {
    Thread? playbackThread;
    lock (_stateLock)
    {
      if (!_playbackRunning)
        return;

      _playbackRunning = false;
      playbackThread = _playbackThread;
    }

    AlsaNative.Drain(_playbackHandle);
    playbackThread?.Join();
  }

  public override void Start()
  {
    lock (_stateLock)
    {
      if (_captureRunning)
        return;

      _captureRunning = true;
      _captureThread = new Thread(CaptureLoop)
      {
        IsBackground = true,
        Name = "alsa-capture"
      };
      _captureThread.Start();
    }
  }

  public override void Stop()
  {
    Thread? captureThread;
    lock (_stateLock)
    {
      if (!_captureRunning)
        return;

      _captureRunning = false;
      captureThread = _captureThread;
    }

    AlsaNative.Drop(_captureHandle);
    captureThread?.Join();
  }

  public override void Dispose()
  {
    if (_disposed)
      return;

    _disposed = true;
    Stop();
    StopPlayback();
    AlsaNative.Close(_captureHandle);
    AlsaNative.Close(_playbackHandle);
  }

  private void CaptureLoop()
  {
    while (_captureRunning)
    {
      var framesRead = AlsaNative.ReadInterleaved(_captureHandle, _captureBuffer, PeriodFrames);
      if (framesRead < 0)
      {
        framesRead = AlsaNative.Recover(_captureHandle, framesRead, 1);
        AlsaNative.ThrowIfError(framesRead, "recovering ALSA capture stream");
        continue;
      }

      var bytesRead = checked((int)framesRead * (int)ChannelCount * BytesPerSample);
      lock (_bufferLock)
      {
        for (var i = 0; i < bytesRead; i++)
          _audioBuffer.Add(_captureBuffer[i]);
      }
    }
  }

  private void PlaybackLoop()
  {
    while (_playbackRunning)
    {
      FillPlaybackBuffer();
      var framesWritten = WriteFullPeriod();
      if (framesWritten < 0)
      {
        framesWritten = AlsaNative.Recover(_playbackHandle, framesWritten, 1);
        AlsaNative.ThrowIfError(framesWritten, "recovering ALSA playback stream");
      }
    }
  }

  private long WriteFullPeriod()
  {
    var totalFramesWritten = 0L;
    while (totalFramesWritten < PeriodFrames && _playbackRunning)
    {
      var frameOffset = (int)totalFramesWritten;
      var framesRemaining = PeriodFrames - frameOffset;
      var byteOffset = frameOffset * (int)ChannelCount * BytesPerSample;
      var byteCount = framesRemaining * (int)ChannelCount * BytesPerSample;
      var tempBuffer = new byte[byteCount];
      Buffer.BlockCopy(_playbackBuffer, byteOffset, tempBuffer, 0, byteCount);

      var framesWritten = AlsaNative.WriteInterleaved(_playbackHandle, tempBuffer, (ulong)framesRemaining);
      if (framesWritten < 0)
        return framesWritten;
      if (framesWritten == 0)
        break;

      totalFramesWritten += framesWritten;
    }

    return totalFramesWritten;
  }

  private void FillPlaybackBuffer()
  {
    Array.Clear(_playbackBuffer);

    lock (_bufferLock)
    {
      var availableDelayedBytes = _audioBuffer.Count - _playbackReadOffset - _delayBytes;
      if (availableDelayedBytes <= 0)
        return;

      var bytesToCopy = Math.Min(_playbackBuffer.Length, availableDelayedBytes);
      for (var i = 0; i < bytesToCopy; i++)
        _playbackBuffer[i] = _audioBuffer[_playbackReadOffset + i];

      _playbackReadOffset += bytesToCopy;

      if (_playbackReadOffset <= _delayBytes || _playbackReadOffset <= _audioBuffer.Count / 2)
        return;

      _audioBuffer.RemoveRange(0, _playbackReadOffset);
      _playbackReadOffset = 0;
    }
  }
}
