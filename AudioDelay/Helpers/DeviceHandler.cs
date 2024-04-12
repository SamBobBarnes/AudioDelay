using System.Text;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using PortAudioSharp;

namespace AudioDelay.Helpers;

public interface IDeviceHandler
{
    int GetInputDeviceCount();
    int GetOutputDeviceCount();
    string GetDevices();
}

public class DeviceHandler : IDeviceHandler
{
    private readonly string _runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;

    public int GetInputDeviceCount()
    {
        switch (_runtime)
        {
            case "win-x64":
                return GetWindowsInputDeviceCount();
            case "linux-x64":
                return GetLinuxInputDeviceCount();
            default:
                return -1;
        }
    }
    
    public int GetOutputDeviceCount()
    {
        switch (_runtime)
        {
            case "win-x64":
                return GetWindowsOutputDeviceCount();
            case "linux-x64":
                return GetLinuxOutputDeviceCount();
            default:
                return -1;
        }
    }
    
    public string GetDevices()
    {
        switch (_runtime)
        {
            case "win-x64":
                return GetWindowsDevices();
            case "linux-x64":
                return GetLinuxDevices();
            default:
                return "No devices available for this runtime.";
        }
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
        
        PortAudio.Initialize();
        
        var deviceCount = PortAudio.DeviceCount;
        
        for (var i = 0; i < deviceCount; i++)
        {
            var deviceInfo = PortAudio.GetDeviceInfo(i);
            stringBuilder.AppendLine($"{i}: {deviceInfo.name}");
        }
        
        return stringBuilder.ToString();
    }
    
    private int GetLinuxInputDeviceCount()
    {
        throw new NotImplementedException();
    }
    
    private int GetLinuxOutputDeviceCount()
    {
        throw new NotImplementedException();
    }
    
    #endregion
    
    
}