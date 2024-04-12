using AudioDelay.Helpers;

namespace AudioDelay.Args;

public class HandleArgs(IDeviceHandler deviceHandler)
{
    public Arguments ParseArgs(string[] args)
    {
        var result = new Arguments();
        var argsList = args.ToList();
        result.Help = ParseHelp(argsList);
        
        var multiplier = 1;
        switch (ParseTimeFormat(argsList))
        {
            case "ms":
                multiplier = 1;
                break;
            case "s":
                multiplier = 1000;
                break;
            case "m":
                multiplier = 60000;
                break;
            case "h":
                multiplier = 3600000;
                break;
        }
        
        result.Delay = ParseDelay(argsList) * multiplier;
        var contentLength = ParseContentLength(argsList) * multiplier;
        if(contentLength < result.Delay) throw new ArgumentException("Content length must be greater or equal to than the delay.");
        result.Runtime = contentLength - result.Delay;
        result.Debug = ParseDebug(argsList);
        result.ListDevices = ParseListDevices(argsList);
        result.InputDevice = ParseInputDevice(argsList);
        result.OutputDevice = ParseOutputDevice(argsList);
        
        return result;
    }

    protected bool ParseHelp(List<string> args)
    {
        if (args.Contains("-h") || args.Contains("--help")) return true;
        return false;
    }

    protected int ParseDelay(List<string> args)
    {
        var delayInt = 5000;
        if (args.Contains("--delay"))
        {
            var index = args.IndexOf("--delay");
            try
            {
                delayInt = int.Parse(args[index + 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("An input following the \"--delay\" option was not found.\nThe \"--delay\" option must be followed by a valid integer");
            }
            catch (Exception)
            {
                throw new ArgumentException("Error parsing --delay input. Should be in the format of \"--delay 5000\"");
            } 
        }

        return delayInt;
    }

    protected int ParseContentLength(List<string> args)
    {
        var runtimeInt = 5000;
        if (args.Contains("--content-length"))
        {
            var index = args.IndexOf("--content-length");
            try
            {
                runtimeInt = int.Parse(args[index + 1]);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("An input following the \"--content-length\" option was not found.\nThe \"--content-length\" option must be followed by a valid integer");
            }
            catch (Exception)
            {
                throw new ArgumentException("Error parsing --content-length input. Should be in the format of \"--content-length 5000\"");
            } 
        }

        return runtimeInt;
    }

    protected string ParseTimeFormat(List<string> args)
    {
        int ms = args.Count(x => x == "--ms");
        int s = args.Count(x => x == "--s");
        int m = args.Count(x => x == "--m");
        int h = args.Count(x => x == "--h");
        
        var totalFlags = ms + s + m + h;
        
        if (totalFlags > 1) throw new ArgumentException("Only one time format flag can be used at a time");
        
        if (ms==1) return "ms";
        if (s==1) return "s";
        if (m==1) return "m";
        if (h==1) return "h";
        
        return "ms";
    }
    
    protected bool ParseDebug(List<string> args)
    {
        if (args.Contains("--debug") || args.Contains("-d")) return true;
        return false;
    }
    
    protected bool ParseListDevices(List<string> args)
    {
        if (args.Contains("--devices")) return true;
        return false;
    }

    protected int ParseInputDevice(List<string> args)
    {
        string flagUsed;
        if (args.Contains("--input-device"))
        {
            flagUsed = "--input-device";
        }
        else if (args.Contains("-i"))
        {
            flagUsed = "-i";
        }
        else
        {
            return 0;
        }
        
        var index = args.IndexOf(flagUsed);
        try
        {
            var inputDevice = int.Parse(args[index + 1]);
            var deviceCount = deviceHandler.GetInputDeviceCount();
            if (inputDevice < 0 || inputDevice > deviceCount) throw new ArgumentOutOfRangeException(nameof(inputDevice),inputDevice.ToString());
            return inputDevice;
        }
        catch (ArgumentOutOfRangeException e)
        {
            if(e.ParamName == "inputDevice")
                throw new ArgumentException($"Invalid input device: {e.Message}");
            
            throw new ArgumentException(
                $"An input following the \"{flagUsed}\" option was not found.\nThe \"{flagUsed}\" option must be followed by a valid integer");
        }
        catch (Exception)
        {
            throw new ArgumentException($"Error parsing {flagUsed} input. Should be in the format of \"{flagUsed} 1\"");
        }
    }
    
    protected int ParseOutputDevice(List<string> args)
    {
        string flagUsed;
        if (args.Contains("--output-device"))
        {
            flagUsed = "--output-device";
        }
        else if (args.Contains("-o"))
        {
            flagUsed = "-o";
        }
        else
        {
            return 0;
        }
        
        var index = args.IndexOf(flagUsed);
        try
        {
            var outputDevice = int.Parse(args[index + 1]);
            var deviceCount = deviceHandler.GetOutputDeviceCount();
            if (outputDevice < 0 || outputDevice > deviceCount) throw new ArgumentOutOfRangeException(nameof(outputDevice),outputDevice.ToString());
            return outputDevice;
        }
        catch (ArgumentOutOfRangeException e)
        {
            if(e.ParamName == "outputDevice")
                throw new ArgumentException($"Invalid output device: {e.Message}");
            
            throw new ArgumentException(
                $"An input following the \"{flagUsed}\" option was not found.\nThe \"{flagUsed}\" option must be followed by a valid integer");
        }
        catch (Exception)
        {
            throw new ArgumentException($"Error parsing {flagUsed} input. Should be in the format of \"{flagUsed} 1\"");
        }
    }

    public string GetHelpText()
    {
        return "Audio Delay v1.0.0\n" +
               "This program allows you to record audio and play it back with a delay.\n" +
               "It utilizes the default audio input and output devices.\n\n" +
               "Options:\n" +
               "--delay [int] - Set the delay in milliseconds\n" +
               "--content-length [int] - Set the content length in milliseconds\n\n" +
               "    Content length must be greater than the delay\n\n" +
               "Time format flags:\n" +
               "    --ms - Set the time format to milliseconds\n" +
               "    --s - Set the time format to seconds\n" +
               "    --m - Set the time format to minutes\n" +
               "    --h - Set the time format to hours\n\n" +
               "    If no time format flag is present, milliseconds will be used\n\n" +
               "--debug - Enable debug mode\n\n\n" +
               "Example usage:\n" +
               "    \"audiodelay --delay 5 --content-length 10 --s\"\n" +
               "    This will record audio for 10 seconds and play it back with a 5 second delay\n" +
               "    The time format is set to seconds\n\n" +
               "    \"audiodelay --delay 5000 --content-length 10000\"\n" +
               "    This will record audio for 10 seconds and play it back with a 5 second delay\n" +
               "    The time format is set to milliseconds\n\n";
    }
}