// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using AudioDelay;

var arguments = HandleArgs.ParseArgs(args);

if (arguments.Help)
{
    Console.Write(HandleArgs.GetHelpText());
    return 0;
}

AudioRecorderPlayer recorderPlayer = new AudioRecorderPlayer(arguments.RecordingLength);
Console.WriteLine("Starting recording and playback...");
recorderPlayer.Start();

Wait(arguments.Delay);
Console.WriteLine("Starting playback...");
recorderPlayer.Play();

Wait(arguments.Runtime);

recorderPlayer.Stop();
Console.WriteLine("Stopped recording.");

Wait(arguments.Delay);
recorderPlayer.StopPlayback();
Console.WriteLine("Stopped playback.");

return 0;

void Wait(int ms)
{
    if (arguments.Debug)
    {
        Debug.WriteLine($"Waiting for {ms} milliseconds...");


        for (int temp = 0; temp < ms; temp += 1000)
        {
            if (ms - temp < 1000)
            {
                Thread.Sleep(ms - temp);
            }
            else
            {
                Thread.Sleep(1000);
            }

            Debug.WriteLine($"Waited for {(double)temp / 1000} seconds...");
        }

        Debug.WriteLine("Done waiting.");
    }
    else
    {
        Thread.Sleep(ms);
    }
}