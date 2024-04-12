using AudioDelay.Args;

namespace AudioDelay.AudioRecorder;

public class MacosRecorder: AudioRecorder
{
    public MacosRecorder(Arguments args) : base(args)
    {
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

    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}