using AudioDelay.Args;
using AudioDelay.AudioRecorder;
using AudioDelay.Helpers;
using Serilog;
using System.Runtime.InteropServices;

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

if (OperatingSystem.IsWindows())
  recorder = new WindowsAudioRecorder(arguments);
else if (OperatingSystem.IsLinux())
  recorder = new LinuxAlsaAudioRecorder(arguments);
else if (OperatingSystem.IsMacOS())
  throw new PlatformNotSupportedException("macOS audio support is not implemented.");
else
{
  Console.WriteLine("Unsupported OS.");
  return 1;
}

var shutdownRequested = 0;
void ShutdownRecorder()
{
  if (Interlocked.Exchange(ref shutdownRequested, 1) == 1)
    return;

  try
  {
    recorder.Shutdown();
  }
  catch (Exception ex)
  {
    Log.Error(ex, "Failed to shut down audio recorder cleanly.");
  }
}

Console.CancelKeyPress += (_, eventArgs) =>
{
  eventArgs.Cancel = true;
  ShutdownRecorder();
};

AppDomain.CurrentDomain.ProcessExit += (_, _) => ShutdownRecorder();

if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
{
  PosixSignalRegistration? sigtermRegistration = null;
  PosixSignalRegistration? sigintRegistration = null;
  sigtermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, _ => ShutdownRecorder());
  sigintRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, _ => ShutdownRecorder());
}

if (arguments.ListDevices)
{
  Console.Write(deviceHandler.GetDevices());
  return 0;
}

if (arguments.TestOutput)
{
  Log.Information("Playing test output...");
  recorder.PlayTestOutput();
  return 0;
}

try
{
  Log.Information("Starting recording...");
  recorder.Start();

  delayHandler.Wait(arguments.Delay, arguments.Debug);

  Log.Information("Starting playback...");
  recorder.Play();

  delayHandler.Wait(arguments.Runtime, arguments.Debug);

  recorder.Stop();
  Log.Information("Stopped recording.");

  delayHandler.Wait(arguments.Delay + arguments.Runtime, arguments.Debug);

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
  ShutdownRecorder();
  recorder.Dispose();
  Log.CloseAndFlush();
}