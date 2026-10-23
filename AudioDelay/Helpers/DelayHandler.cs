using Serilog;

namespace AudioDelay.Helpers;

public class DelayHandler(IThreadHandler threadHandler)
{
  public Task Schedule(int ms, bool debug, Action? callback = null)
  {
    return Task.Run(async () =>
    {
      if (debug)
      {
        Log.Debug($"Waiting for {ms} milliseconds...");

        for (var temp = 0; temp < ms; temp += 1000)
        {
          var sleepMs = ms - temp < 1000 ? ms - temp : 1000;
          await Task.Delay(sleepMs);
          Log.Debug($"Waited for {(double)temp / 1000 + 1} seconds...");
          temp += sleepMs;
        }

        Log.Debug("Done waiting.");
      }
      else
      {
        await Task.Delay(ms);
      }

      callback?.Invoke();
    });
  }

  public void Wait(int ms, bool debug, Action? callback = null)
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

    callback?.Invoke();
  }
}