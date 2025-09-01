using Serilog;

namespace AudioDelay.Helpers;

public class DelayHandler(IThreadHandler threadHandler)
{
  public void Wait(int ms, bool debug)
  {
    if (debug)
    {
      Log.Information("Waiting for {Ms} milliseconds...", ms);

      for (var temp = 0; temp < ms; temp += 1000)
      {
        if (ms - temp < 1000)
          threadHandler.Sleep(ms - temp);
        else
          threadHandler.Sleep(1000);

        Log.Information("Waited for {Temp} seconds...", (double)temp / 1000 + 1);
      }

      Log.Information("Done waiting");
    }
    else
      threadHandler.Sleep(ms);
  }
}