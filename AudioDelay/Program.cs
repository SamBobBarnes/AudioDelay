using System.Diagnostics;
using AudioDelay;
using AudioDelay.Args;
using AudioDelay.AudioRecorder;
using AudioDelay.Helpers;

var delayHandler = new DelayHandler(new ThreadHandler());

var arguments = HandleArgs.ParseArgs(args);

if (arguments.Help)
{
    Console.Write(HandleArgs.GetHelpText());
    return 0;
}

AudioRecorder recorder = new AudioRecorder(arguments.RecordingLength);

Console.WriteLine("Starting recording and playback...");
recorder.Start();

delayHandler.Wait(arguments.Delay, arguments.Debug);

Console.WriteLine("Starting playback...");
recorder.Play();

delayHandler.Wait(arguments.Runtime, arguments.Debug);

recorder.Stop();
Console.WriteLine("Stopped recording.");

delayHandler.Wait(arguments.Delay, arguments.Debug);

recorder.StopPlayback();
Console.WriteLine("Stopped playback.");

return 0;