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

IAudioRecorder recorder;

var runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;

switch (runtime)
{
    case "win-x64":
        recorder = new WindowsAudioRecorder(arguments.RecordingLength);
        break;
    default:
        Console.WriteLine("Unsupported OS.");
        return 1;
}

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