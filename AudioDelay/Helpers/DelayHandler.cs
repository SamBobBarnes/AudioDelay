using Serilog;

namespace AudioDelay.Helpers;

public class DelayHandler(IThreadHandler threadHandler)
{
  public void Wait(int ms, bool debug)
  {
    if (debug)
    {
      Log.Debug($"Waiting for {ms} milliseconds...");

      for (var temp = 0; temp < ms; temp += 1000)
      {
        if (ms - temp < 1000)
          threadHandler.Sleep(ms - temp);
        else
          threadHandler.Sleep(1000);

        Log.Debug($"Waited for {(double)temp / 1000 + 1} seconds...");
      }

      Log.Debug("Done waiting.");
    }
    else
      threadHandler.Sleep(ms);
  }
}