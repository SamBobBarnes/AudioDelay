namespace AudioDelay.Helpers;

public class DelayHandler(IThreadHandler threadHandler)
{
    public void Wait(int ms, bool debug)
    {
        if (debug)
        {
            Console.WriteLine($"Waiting for {ms} milliseconds...");


            for (int temp = 0; temp < ms; temp += 1000)
            {
                if (ms - temp < 1000)
                {
                    threadHandler.Sleep(ms - temp);
                }
                else
                {
                    threadHandler.Sleep(1000);
                }

                Console.WriteLine($"Waited for {(double)temp / 1000 + 1} seconds...");
            }

            Console.WriteLine("Done waiting.");
        }
        else
        {
            threadHandler.Sleep(ms);
        }
    }
}