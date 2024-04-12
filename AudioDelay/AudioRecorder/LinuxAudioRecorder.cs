using AudioDelay.Args;
using PortAudioSharp;

namespace AudioDelay.AudioRecorder;

public class LinuxAudioRecorder: AudioRecorder
{
    public LinuxAudioRecorder(Arguments args) : base(args)
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
}