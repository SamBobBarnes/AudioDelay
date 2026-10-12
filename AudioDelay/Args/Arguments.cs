namespace AudioDelay.Args;

public class Arguments
{
  public bool Help { get; set; }
  public bool TestOutput { get; set; }
  public int Delay { get; set; }
  public int ContentLength { get; set; }
  public int RecordingLength => ContentLength;
  public bool Debug { get; set; }
  public bool ListDevices { get; set; }
  public int InputDevice { get; set; }
  public int OutputDevice { get; set; }
  public string LoggerName { get; set; } = "";
  public Uri? LoggerUrl { get; set; }
}

public static class ValidLoggers
{
  public const string Loki = "loki";
  public const string Elasticsearch = "elasticsearch";
}