package Args;

public class Arguments
{
    public boolean Help;
    public int Delay;
    public int Runtime;
    public int getRecordingLength() {
        return Delay + Runtime;
    }
    public boolean Debug;
    public boolean ListDevices;
    public int InputDevice = 0;
    public int OutputDevice = 0;
}