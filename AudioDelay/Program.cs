using System.Diagnostics;
using AudioDelay;
using AudioDelay.Helpers;

var delayHandler = new DelayHandler(new ThreadHandler());

var arguments = HandleArgs.ParseArgs(args);

if (arguments.Help)
{
    Console.Write(HandleArgs.GetHelpText());
    return 0;
}

AudioRecorderPlayer recorderPlayer = new AudioRecorderPlayer(arguments.RecordingLength);

Console.WriteLine("Starting recording and playback...");
recorderPlayer.Start();

delayHandler.Wait(arguments.Delay, arguments.Debug);

Console.WriteLine("Starting playback...");
recorderPlayer.Play();

delayHandler.Wait(arguments.Runtime, arguments.Debug);

recorderPlayer.Stop();
Console.WriteLine("Stopped recording.");

delayHandler.Wait(arguments.Delay, arguments.Debug);

recorderPlayer.StopPlayback();
Console.WriteLine("Stopped playback.");

return 0;