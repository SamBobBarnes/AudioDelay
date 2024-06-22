using AudioDelay.Args;
using AudioDelay.Helpers;
using FluentAssertions;
using NSubstitute;

namespace Tests;

public class HandleArgsTests() : HandleArgs(_deviceHandler)
{
  private static readonly IDeviceHandler _deviceHandler = Substitute.For<IDeviceHandler>();

  #region HandleHelp

  [Fact]
  public void ParseHelp_ShouldReturnFalseByDefault()
  {
    var args = new List<string>();

    var actual = ParseHelp(args);

    actual.Should().BeFalse();
  }

  [Fact]
  public void ParseHelp_ShouldReturnTrueWhenHelpFlagIsPresent_Short()
  {
    var args = new List<string> { "-h" };

    var actual = ParseHelp(args);

    actual.Should().BeTrue();
  }

  [Fact]
  public void ParseHelp_ShouldReturnTrueWhenHelpFlagIsPresent_Long()
  {
    var args = new List<string> { "--help" };

    var actual = ParseHelp(args);

    actual.Should().BeTrue();
  }

  #endregion

  #region HandleDelay

  [Fact]
  public void ParseDelay_ShouldReturn5000ByDefault()
  {
    var args = new List<string>();

    var actual = ParseDelay(args);

    actual.Should().Be(5000);
  }

  [Fact]
  public void ParseDelay_ShouldReturnInputWhenDelayFlagIsPresent()
  {
    var args = new List<string> { "--delay", "2000" };

    var actual = ParseDelay(args);

    actual.Should().Be(2000);
  }

  [Fact]
  public void ParseDelay_ShouldThrowExceptionWhenInputIsMissing()
  {
    var args = new List<string> { "--delay" };

    Action act = () => ParseDelay(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        "An input following the \"--delay\" option was not found.\nThe \"--delay\" option must be followed by a valid integer");
  }

  [Fact]
  public void ParseDelay_ShouldThrowExceptionWhenInputIsNotAnInteger()
  {
    var args = new List<string> { "--delay", "two" };

    Action act = () => ParseDelay(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage("Error parsing --delay input. Should be in the format of \"--delay 5000\"");
  }

  #endregion

  #region ParseContentLength

  [Fact]
  public void ParseContentLength_ShouldReturn5000ByDefault()
  {
    var args = new List<string>();

    var actual = ParseContentLength(args);

    actual.Should().Be(5000);
  }

  [Fact]
  public void ParseContentLength_ShouldReturnInputWhenContentLengthFlagIsPresent()
  {
    var args = new List<string> { "--content-length", "2000" };

    var actual = ParseContentLength(args);

    actual.Should().Be(2000);
  }

  [Fact]
  public void ParseContentLength_ShouldThrowExceptionWhenInputIsMissing()
  {
    var args = new List<string> { "--content-length" };

    Action act = () => ParseContentLength(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        "An input following the \"--content-length\" option was not found.\nThe \"--content-length\" option must be followed by a valid integer");
  }

  [Fact]
  public void ParseContentLength_ShouldThrowExceptionWhenInputIsNotAnInteger()
  {
    var args = new List<string> { "--content-length", "two" };

    Action act = () => ParseContentLength(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage("Error parsing --content-length input. Should be in the format of \"--content-length 5000\"");
  }

  #endregion

  #region ParseTimeFormat

  [Fact]
  public void ParseTimeFormat_ShouldReturn_ms_ByDefault()
  {
    var args = new List<string>();

    var actual = ParseTimeFormat(args);

    actual.Should().Be("ms");
  }

  [Fact]
  public void ParseTimeFormat_ShouldReturn_ms_WhenMsFlagIsPresent()
  {
    var args = new List<string> { "--ms" };

    var actual = ParseTimeFormat(args);

    actual.Should().Be("ms");
  }

  [Fact]
  public void ParseTimeFormat_ShouldReturn_s_WhenSFlagIsPresent()
  {
    var args = new List<string> { "--s" };

    var actual = ParseTimeFormat(args);

    actual.Should().Be("s");
  }

  [Fact]
  public void ParseTimeFormat_ShouldReturn_m_WhenMFlagIsPresent()
  {
    var args = new List<string> { "--m" };

    var actual = ParseTimeFormat(args);

    actual.Should().Be("m");
  }

  [Fact]
  public void ParseTimeFormat_ShouldReturn_h_WhenHFlagIsPresent()
  {
    var args = new List<string> { "--h" };

    var actual = ParseTimeFormat(args);

    actual.Should().Be("h");
  }

  [Theory]
  [CombinatorialData]
  public void ParseTimeFormat_ShouldThrowExceptionWhenMoreThanOneTimeFlagIsPresent(
    [CombinatorialValues("--ms", "--s", "--m", "--h")]
    string flag1,
    [CombinatorialValues("--ms", "--s", "--m", "--h")]
    string flag2
  )
  {
    var args = new List<string> { flag1, flag2 };

    Action act = () => ParseTimeFormat(args);

    act.Should().Throw<ArgumentException>().WithMessage("Only one time format flag can be used at a time");
  }

  #endregion

  #region ParseDebug

  [Fact]
  public void ParseDebug_ShouldReturnFalseByDefault()
  {
    var args = new List<string>();

    var actual = ParseDebug(args);

    actual.Should().BeFalse();
  }

  [Fact]
  public void ParseDebug_ShouldReturnTrueWhenDebugFlagIsPresent()
  {
    var args = new List<string> { "--debug" };

    var actual = ParseDebug(args);

    actual.Should().BeTrue();
  }

  [Fact]
  public void ParseDebug_ShouldReturnTrueWhenDebugFlagIsPresent_Short()
  {
    var args = new List<string> { "-d" };

    var actual = ParseDebug(args);

    actual.Should().BeTrue();
  }

  #endregion

  #region ParseListDevices

  [Fact]
  public void ParseListDevices_ShouldReturnFalseByDefault()
  {
    var args = new List<string>();

    var actual = ParseListDevices(args);

    actual.Should().BeFalse();
  }

  [Fact]
  public void ParseListDevices_ShouldReturnTrueWhenDevicesFlagIsPresent()
  {
    var args = new List<string> { "--devices" };

    var actual = ParseListDevices(args);

    actual.Should().BeTrue();
  }

  #endregion

  #region ParseInputDevice

  [Fact]
  public void ParseInputDevice_ShouldReturnZeroByDefault()
  {
    var args = new List<string>();
    _deviceHandler.GetInputDeviceCount().Returns(1);

    var actual = ParseInputDevice(args);

    actual.Should().Be(0);
  }

  [Fact]
  public void ParseInputDevice_ShouldReturnInputWhenInputDeviceFlagIsPresent()
  {
    var args = new List<string> { "--input-device", "1" };
    _deviceHandler.GetInputDeviceCount().Returns(1);

    var actual = ParseInputDevice(args);

    actual.Should().Be(1);
  }

  [Fact]
  public void ParseInputDevice_ShouldReturnInputWhenInputDeviceFlagIsPresent_Short()
  {
    var args = new List<string> { "-i", "1" };
    _deviceHandler.GetInputDeviceCount().Returns(1);

    var actual = ParseInputDevice(args);

    actual.Should().Be(1);
  }

  [Theory]
  [CombinatorialData]
  public void ParseInputDevice_ShouldThrowOutOfRangeExceptionWhenInputIsNotACurrentDevice(
    [CombinatorialValues(-1, 2)] int device
  )
  {
    var args = new List<string> { "--input-device", device.ToString() };
    _deviceHandler.GetInputDeviceCount().Returns(1);

    Action act = () => ParseInputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage($"Invalid input device: {device.ToString()} (Parameter 'inputDevice')");
  }

  [Theory]
  [InlineData("--input-device")]
  [InlineData("-i")]
  public void ParseInputDevice_ShouldThrowExceptionWhenInputIsMissing(string flag)
  {
    var args = new List<string> { flag };

    Action act = () => ParseInputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        $"An input following the \"{flag}\" option was not found.\nThe \"{flag}\" option must be followed by a valid integer");
  }

  [Theory]
  [InlineData("--input-device")]
  [InlineData("-i")]
  public void ParseInputDevice_ShouldThrowExceptionWhenInputIsNotAnInteger(string flag)
  {
    var args = new List<string> { flag, "two" };

    Action act = () => ParseInputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage($"Error parsing {flag} input. Should be in the format of \"{flag} 1\"");
  }

  #endregion

  #region ParseOutputDevice

  [Fact]
  public void ParseOutputDevice_ShouldReturnZeroByDefault()
  {
    var args = new List<string>();
    _deviceHandler.GetOutputDeviceCount().Returns(1);

    var actual = ParseOutputDevice(args);

    actual.Should().Be(0);
  }

  [Fact]
  public void ParseOutputDevice_ShouldReturnInputWhenOutputDeviceFlagIsPresent()
  {
    var args = new List<string> { "--output-device", "1" };
    _deviceHandler.GetOutputDeviceCount().Returns(1);

    var actual = ParseOutputDevice(args);

    actual.Should().Be(1);
  }

  [Fact]
  public void ParseOutputDevice_ShouldReturnInputWhenOutputDeviceFlagIsPresent_Short()
  {
    var args = new List<string> { "-o", "1" };
    _deviceHandler.GetOutputDeviceCount().Returns(1);

    var actual = ParseOutputDevice(args);

    actual.Should().Be(1);
  }

  [Theory]
  [CombinatorialData]
  public void ParseOutputDevice_ShouldThrowOutOfRangeExceptionWhenInputIsNotACurrentDevice(
    [CombinatorialValues(-1, 1)] int device
  )
  {
    var args = new List<string> { "--output-device", device.ToString() };
    _deviceHandler.GetOutputDeviceCount().Returns(1);

    Action act = () => ParseOutputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage($"Invalid output device: {device.ToString()} (Parameter 'outputDevice')");
  }

  [Theory]
  [InlineData("--output-device")]
  [InlineData("-o")]
  public void ParseOutputDevice_ShouldThrowExceptionWhenInputIsMissing(string flag)
  {
    var args = new List<string> { flag };

    Action act = () => ParseOutputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        $"An input following the \"{flag}\" option was not found.\nThe \"{flag}\" option must be followed by a valid integer");
  }

  [Theory]
  [InlineData("--output-device")]
  [InlineData("-o")]
  public void ParseOutputDevice_ShouldThrowExceptionWhenInputIsNotAnInteger(string flag)
  {
    var args = new List<string> { flag, "two" };

    Action act = () => ParseOutputDevice(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage($"Error parsing {flag} input. Should be in the format of \"{flag} 1\"");
  }

  #endregion

  #region ParseArgs

  [Fact]
  public void ParseArgs_ShouldReturnDefaultValuesWhenNoArgsArePassed()
  {
    var args = Array.Empty<string>();

    var actual = ParseArgs(args);

    actual.Should().BeEquivalentTo(new Arguments { Delay = 5000, Runtime = 0, Help = false, Debug = false });
  }

  [Fact]
  public void ParseArgs_ShouldReturnValuesWhenArgsArePassed()
  {
    _deviceHandler.GetInputDeviceCount().Returns(3);
    _deviceHandler.GetOutputDeviceCount().Returns(3);

    var args = new[]
    {
      "--delay",
      "2000",
      "--content-length",
      "3000",
      "--help",
      "--debug",
      "--input-device",
      "1",
      "--output-device",
      "2",
      "--devices"
    };

    var actual = ParseArgs(args);

    actual.Should()
      .BeEquivalentTo(new Arguments
      {
        Delay = 2000,
        Runtime = 1000,
        Help = true,
        Debug = true,
        InputDevice = 1,
        OutputDevice = 2,
        ListDevices = true
      });
  }

  [Theory]
  [InlineData("--ms", 3, 1)]
  [InlineData("--s", 3000, 1000)]
  [InlineData("--m", 180000, 60000)]
  [InlineData("--h", 10800000, 3600000)]
  public void ParseArgs_ShouldConvertTimeWhenTimeFlagIsPresent(string flag, int delayResult, int runtimeResult)
  {
    var args = new[] { "--delay", "3", "--content-length", "4", flag };

    var actual = ParseArgs(args);

    actual.Should().BeEquivalentTo(new Arguments { Delay = delayResult, Runtime = runtimeResult, Help = false });
  }

  [Fact]
  public void ParseArgs_ShouldThrowExceptionWhenContentLengthIsLessThanDelay()
  {
    var args = new[] { "--delay", "3000", "--content-length", "2000" };

    Action act = () => ParseArgs(args);

    act.Should().Throw<ArgumentException>().WithMessage("Content length must be greater or equal to than the delay.");
  }

  #endregion

  #region ParseLoggers

  [Fact]
  public void ParseLoggers_ShouldReturnEmptyStringByDefault()
  {
    var args = new List<string>();

    var actual = ParseLoggers(args);

    actual.Item1.Should().BeEmpty();
    actual.Item2.Should().BeNull();
  }

  [Theory]
  [CombinatorialData]
  public void ParseLoggers_ShouldThrowExceptionWhenMultipleFlagsPresent(
    [CombinatorialValues("--logger", "-l")]
    string flag1,
    [CombinatorialValues("--logger", "-l")]
    string flag2
  )
  {
    var args = new List<string> { flag1, flag2 };

    Action act = () => ParseLoggers(args);

    act.Should().Throw<ArgumentException>().WithMessage("Only one logger flag can be used at a time!");
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldReturnLokiAndUrl(string flag)
  {
    var actual = ParseLoggers(new List<string> { flag, "loki;http://localhost:3100" });

    actual.Item1.Should().Be(ValidLoggers.Loki);
    actual.Item2.Should().Be(new Uri("http://localhost:3100"));
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldReturnElasticsearchAndUrl(string flag)
  {
    var actual = ParseLoggers(new List<string> { flag, "elasticsearch;http://localhost:9200" });

    actual.Item1.Should().Be(ValidLoggers.Elasticsearch);
    actual.Item2.Should().Be(new Uri("http://localhost:9200"));
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldThrowExceptionWhenLoggerNameIsInvalid(string flag)
  {
    var args = new List<string> { flag, "invalid;http://localhost:3100" };

    Action act = () => ParseLoggers(args);

    act.Should().Throw<ArgumentException>().WithMessage("Invalid logger name");
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldThrowExceptionWhenUrlIsInvalid(string flag)
  {
    var args = new List<string> { flag, "loki;invalid" };

    Action act = () => ParseLoggers(args);

    act.Should().Throw<ArgumentException>().WithMessage("Invalid logger URI");
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldThrowExceptionWhenSecondParamMissing(string flag)
  {
    var args = new List<string> { flag };

    Action act = () => ParseLoggers(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        $"The \"{flag}\" option must be in the format of \"{flag} [loki|elasticsearch];[URI]\"");
  }

  [Theory]
  [InlineData("--logger")]
  [InlineData("-l")]
  public void ParseLoggers_ShouldThrowExceptionWhenUrlIsMissing(string flag)
  {
    var args = new List<string> { flag, "loki" };

    Action act = () => ParseLoggers(args);

    act.Should()
      .Throw<ArgumentException>()
      .WithMessage(
        $"An input following the \"{flag}\" option was not found.\nThe \"{flag}\" option must be in the format of \"{flag} [loki|elasticsearch];[URI]\"");
  }

  #endregion
}