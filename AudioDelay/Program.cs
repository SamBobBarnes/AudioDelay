using AudioDelay.Args;
using AudioDelay.AudioRecorder;
using AudioDelay.Helpers;

var runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;

var deviceHandler = new DeviceHandler();

var delayHandler = new DelayHandler(new ThreadHandler());

var handleArgs = new HandleArgs(deviceHandler);

var arguments = handleArgs.ParseArgs(args);

if (arguments.Help)
{
    Console.Write(handleArgs.GetHelpText());
    return 0;
}

IAudioRecorder recorder;

switch (runtime)
{
    case "win-x64":
        recorder = new WindowsAudioRecorder(arguments);
        break;
    default:
        Console.WriteLine("Unsupported OS.");
        return 1;
}

if (arguments.ListDevices)
{
    Console.Write(deviceHandler.GetDevices());
    return 0;
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