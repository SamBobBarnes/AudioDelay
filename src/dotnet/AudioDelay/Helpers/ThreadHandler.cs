namespace AudioDelay.Helpers;

public interface IThreadHandler
{
    void Sleep(int ms);
}

public class ThreadHandler : IThreadHandler
{
    public void Sleep(int ms)
    {
        Thread.Sleep(ms);
    }
}

