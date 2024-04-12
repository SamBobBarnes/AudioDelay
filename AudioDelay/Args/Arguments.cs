namespace AudioDelay.Args;

public class Arguments
{
    public bool Help { get; set; }
    public int Delay { get; set; }
    public int Runtime { get; set; }
    public int RecordingLength => Delay + Runtime;
    public bool Debug { get; set; }
    public bool ListDevices { get; set; }
    public int InputDevice { get; set; } = 0;
    public int OutputDevice { get; set; } = 0;
}