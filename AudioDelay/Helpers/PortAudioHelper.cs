using PortAudioSharp;

namespace AudioDelay.Helpers;

public static class PortAudioHelper
{
  public static void PrintDevices()
  {
    PrintOutputDevices();
    PrintInputDevices();
  }

  private static DeviceInfo[] GetDevices()
  {
    var deviceCount = PortAudio.DeviceCount;
    var devices = new DeviceInfo[deviceCount];

    for (var i = 0; i < deviceCount; i++)
    {
      devices[i] = PortAudio.GetDeviceInfo(i);
    }

    return devices;
  }

  public static void PrintDefaultDevices()
  {
    Console.WriteLine("Default Input Device: " + PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice));
    Console.WriteLine("Default Output Device: " + PortAudio.GetDeviceInfo(PortAudio.DefaultOutputDevice));
  }

  public static void PrintOutputDevices()
  {
    var devices = GetDevices();
    var outputDevices = devices.Where(d => d.maxOutputChannels > 0).ToArray();

    Console.WriteLine("Number of Output Devices: " + outputDevices.Length);
    Console.WriteLine($"{"#",5} | {"Device Name",-50} | {"Output Channels",15} | {"Default Sample Rate",20}");
    for (var i = 0; i < outputDevices.Length; i++)
    {
      var device = outputDevices[i];
      if (device.maxOutputChannels > 0)
      {
        Console.WriteLine(
          $"{i,5} | {device.name.Substring(0, Math.Min(50, device.name.Length)),-50} | {device.maxOutputChannels,15} | {device.defaultSampleRate,20}");
      }
    }
  }

  public static void PrintInputDevices()
  {
    var devices = GetDevices();
    var inputDevices = devices.Where(d => d.maxInputChannels > 0).ToArray();

    Console.WriteLine("Number of Input Devices: " + inputDevices.Length);
    Console.WriteLine($"{"#",5} | {"Device Name",-50} | {"Input Channels",15} | {"Default Sample Rate",20}");
    for (var i = 0; i < inputDevices.Length; i++)
    {
      var device = inputDevices[i];
      if (device.maxInputChannels > 0)
      {
        Console.WriteLine(
          $"{i,5} | {device.name.Substring(0, Math.Min(50, device.name.Length)),-50} | {device.maxInputChannels,15} | {device.defaultSampleRate,20}");
      }
    }
  }
}