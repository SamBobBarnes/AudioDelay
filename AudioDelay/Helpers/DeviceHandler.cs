using System.Text;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace AudioDelay.Helpers;

public interface IDeviceHandler
{
    int GetInputDeviceCount();
    int GetOutputDeviceCount();
    string GetDevices();
    bool IsDefaultDeviceIndex(int deviceIndex);
}

public class DeviceHandler : IDeviceHandler
{
    public int GetInputDeviceCount()
    {
        if (OperatingSystem.IsWindows())
            return GetWindowsInputDeviceCount();
        if (OperatingSystem.IsLinux())
            return 1;
        if (OperatingSystem.IsMacOS())
            return 0;

        return -1;
    }
    
    public int GetOutputDeviceCount()
    {
        if (OperatingSystem.IsWindows())
            return GetWindowsOutputDeviceCount();
        if (OperatingSystem.IsLinux())
            return 1;
        if (OperatingSystem.IsMacOS())
            return 0;

        return -1;
    }
    
    public string GetDevices()
    {
        if (OperatingSystem.IsWindows())
            return GetWindowsDevices();
        if (OperatingSystem.IsLinux())
            return GetLinuxDevices();
        if (OperatingSystem.IsMacOS())
            return "No devices available for this runtime.";

        return "No devices available for this runtime.";
    }

    public bool IsDefaultDeviceIndex(int deviceIndex)
    {
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            return deviceIndex is -1 or 0;

        return deviceIndex == 0;
    }
    
    #region Windows
    private string GetWindowsDevices()
    {
        var enumerator = new MMDeviceEnumerator();
        var stringBuilder = new StringBuilder();
        
        var inputDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();

        stringBuilder.AppendLine("Input devices:");
        for(var i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            var deviceName = inputDevices.First(x => x.FriendlyName.StartsWith(WaveInEvent.GetCapabilities(i).ProductName)).FriendlyName;
            stringBuilder.AppendLine($"{i}: {deviceName}");
        }
        
        var index = 1;
        
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Playback devices:");
        stringBuilder.AppendLine("0: Primary Output Device (Default)");
        foreach (var endpoint in
                 enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
        {
            stringBuilder.AppendLine($"{index}: {endpoint.FriendlyName}");
            index++;
        }
        
        return stringBuilder.ToString();
    }
    
    private int GetWindowsInputDeviceCount()
    {
        var enumerator = new MMDeviceEnumerator();

        return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Count;
    }
    
    private int GetWindowsOutputDeviceCount()
    {
        var enumerator = new MMDeviceEnumerator();

        return enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).Count;
    }
    
    #endregion

    #region Linux

    private string GetLinuxDevices()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("Input devices:");
        stringBuilder.AppendLine("0: default");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Playback devices:");
        stringBuilder.AppendLine("0: default");
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("Linux currently uses the ALSA default capture and playback devices.");
        return stringBuilder.ToString();
    }

    #endregion
}