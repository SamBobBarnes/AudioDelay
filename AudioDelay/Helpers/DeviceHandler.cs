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
    bool IsDefaultDeviceIndex(int deviceIndex);
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
            case "linux-arm64":
            case "osx-x64":
            case "osx-arm64":
                return GetPortAudioInputDeviceCount();
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
            case "linux-arm64":
            case "osx-x64":
            case "osx-arm64":
                return GetPortAudioOutputDeviceCount();
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
            case "linux-arm64":
            case "osx-x64":
            case "osx-arm64":
                return GetPortAudioDevices();
            default:
                return "No devices available for this runtime.";
        }
    }

    public bool IsDefaultDeviceIndex(int deviceIndex)
    {
        switch (_runtime)
        {
            case "linux-x64":
            case "linux-arm64":
            case "osx-x64":
            case "osx-arm64":
                return deviceIndex == -1;
            default:
                return deviceIndex == 0;
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

    #region PortAudio

    private string GetPortAudioDevices()
    {
        PortAudio.Initialize();
        try
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Input devices:");
            stringBuilder.AppendLine($"-1: Default Input Device ({PortAudio.GetDeviceInfo(PortAudio.DefaultInputDevice).name})");
            for (var i = 0; i < PortAudio.DeviceCount; i++)
            {
                var device = PortAudio.GetDeviceInfo(i);
                if (device.maxInputChannels > 0)
                    stringBuilder.AppendLine($"{i}: {device.name}");
            }

            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Playback devices:");
            stringBuilder.AppendLine($"-1: Default Output Device ({PortAudio.GetDeviceInfo(PortAudio.DefaultOutputDevice).name})");
            for (var i = 0; i < PortAudio.DeviceCount; i++)
            {
                var device = PortAudio.GetDeviceInfo(i);
                if (device.maxOutputChannels > 0)
                    stringBuilder.AppendLine($"{i}: {device.name}");
            }

            return stringBuilder.ToString();
        }
        finally
        {
            PortAudio.Terminate();
        }
    }

    private int GetPortAudioInputDeviceCount()
    {
        PortAudio.Initialize();
        try
        {
            return Enumerable.Range(0, PortAudio.DeviceCount)
                .Count(index => PortAudio.GetDeviceInfo(index).maxInputChannels > 0);
        }
        finally
        {
            PortAudio.Terminate();
        }
    }

    private int GetPortAudioOutputDeviceCount()
    {
        PortAudio.Initialize();
        try
        {
            return Enumerable.Range(0, PortAudio.DeviceCount)
                .Count(index => PortAudio.GetDeviceInfo(index).maxOutputChannels > 0);
        }
        finally
        {
            PortAudio.Terminate();
        }
    }

    #endregion
}