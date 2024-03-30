namespace AudioDelay;

public class HandleArgs
{
    protected HandleArgs() {}

    public static Arguments ParseArgs(string[] args)
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
        result.Runtime = ParseRuntime(argsList) * multiplier;
        
        return result;
    }

    protected static bool ParseHelp(List<string> args)
    {
        if (args.Contains("-h") || args.Contains("--help")) return true;
        return false;
    }

    protected static int ParseDelay(List<string> args)
    {
        var delayInt = 5000;
        if (args.Contains("--delay"))
        {
            var index = args.IndexOf("--delay");
            try
            {
                delayInt = int.Parse(args[index + 1]);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new Exception("An input following the \"--delay\" option was not found.\nThe \"--delay\" option must be followed by a valid integer");
            }
            catch (Exception e)
            {
                throw new Exception("Error parsing --delay input. Should be in the format of \"--delay 5000\"");
            } 
        }

        return delayInt;
    }

    protected static int ParseRuntime(List<string> args)
    {
        var runtimeInt = 5000;
        if (args.Contains("--runtime"))
        {
            var index = args.IndexOf("--runtime");
            try
            {
                runtimeInt = int.Parse(args[index + 1]);
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new Exception("An input following the \"--runtime\" option was not found.\nThe \"--runtime\" option must be followed by a valid integer");
            }
            catch (Exception e)
            {
                throw new Exception("Error parsing --runtime input. Should be in the format of \"--runtime 5000\"");
            } 
        }

        return runtimeInt;
    }

    protected static string ParseTimeFormat(List<string> args)
    {
        int ms = args.Count(x => x == "--ms");
        int s = args.Count(x => x == "--s");
        int m = args.Count(x => x == "--m");
        int h = args.Count(x => x == "--h");
        
        var totalFlags = ms + s + m + h;
        
        if (totalFlags > 1) throw new Exception("Only one time format flag can be used at a time");
        
        if (ms==1) return "ms";
        if (s==1) return "s";
        if (m==1) return "m";
        if (h==1) return "h";
        
        return "ms";
    }

    public static string GetHelpText()
    {
        return "Audio Delay v1.0.0\n" +
               "This program allows you to record audio and play it back with a delay.\n" +
               "It utilizes the default audio input and output devices.\n\n" +
               "Options:\n" +
               "--delay [int] - Set the delay in milliseconds\n" +
               "--runtime [int] - Set the runtime in milliseconds\n\n" +
               "Time format flags:\n" +
               "    --ms - Set the time format to milliseconds\n" +
               "    --s - Set the time format to seconds\n" +
               "    --m - Set the time format to minutes\n" +
               "    --h - Set the time format to hours\n\n" +
               "    If no time format flag is present, milliseconds will be used\n\n" +
               "Example usage:\n" +
               "    \"audiodelay --delay 5 --runtime 10 --s\"\n" +
               "    This will record audio for 10 seconds and play it back with a 5 second delay\n" +
               "    The time format is set to seconds\n\n" +
               "    \"audiodelay --delay 5000 --runtime 10000\"\n" +
               "    This will record audio for 10 seconds and play it back with a 5 second delay\n" +
               "    The time format is set to milliseconds\n\n";
    }
}