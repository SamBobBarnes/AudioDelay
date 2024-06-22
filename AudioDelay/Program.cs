using System.Runtime.InteropServices;
using AudioDelay.Args;
using AudioDelay.AudioRecorder;
using AudioDelay.Helpers;
using Serilog;

var runtime = RuntimeInformation.RuntimeIdentifier;

var deviceHandler = new DeviceHandler();

var delayHandler = new DelayHandler(new ThreadHandler());

var handleArgs = new HandleArgs(deviceHandler);

var arguments = handleArgs.ParseArgs(args);

if (arguments.Help)
{
  Console.Write(handleArgs.GetHelpText());
  return 0;
}

Log.Logger = LoggerConfigurator.ConfigureLogger(arguments).CreateLogger();

AudioRecorder recorder;

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

try
{
  Log.Information("Starting recording and playback...");
  recorder.Start();

  delayHandler.Wait(arguments.Delay, arguments.Debug);

  Log.Information("Starting playback...");
  recorder.Play();

  delayHandler.Wait(arguments.Runtime, arguments.Debug);

  recorder.Stop();
  Log.Information("Stopped recording.");

  delayHandler.Wait(arguments.Delay, arguments.Debug);

  recorder.StopPlayback();
  Log.Information("Stopped playback.");

  return 0;
}
catch (Exception ex)
{
  Log.Error(ex, "An error occurred.");
  return 1;
}
finally
{
  Log.CloseAndFlush();
}

{ }