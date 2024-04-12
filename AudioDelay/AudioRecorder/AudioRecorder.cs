using AudioDelay.Args;

namespace AudioDelay.AudioRecorder;

public abstract class AudioRecorder
{
  protected AudioRecorder(Arguments args) {}

  public abstract void Play();
  public abstract void StopPlayback();
  public abstract void Start();
  public abstract void Stop();
}