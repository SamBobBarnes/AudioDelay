package AudioRecorder;

import Args.Arguments;

import javax.sound.sampled.AudioInputStream;

public abstract class AudioRecorder
{
    protected Arguments _args;
    protected AudioRecorder(Arguments args) {
        _args = args;
    }
    public abstract void Play();
    public abstract void StopPlayback();
    public abstract void Start();
    public abstract void Stop();
}
