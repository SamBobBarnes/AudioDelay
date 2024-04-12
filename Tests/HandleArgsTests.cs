using AudioDelay;
using AudioDelay.Args;
using FluentAssertions;

namespace Tests;

public class HandleArgsTests : HandleArgs
{

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

        act.Should().Throw<Exception>().WithMessage("An input following the \"--delay\" option was not found.\nThe \"--delay\" option must be followed by a valid integer");
    }
    
    [Fact]
    public void ParseDelay_ShouldThrowExceptionWhenInputIsNotAnInteger()
    {
        var args = new List<string> { "--delay", "two" };

        Action act = () => ParseDelay(args);

        act.Should().Throw<Exception>().WithMessage("Error parsing --delay input. Should be in the format of \"--delay 5000\"");
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

        act.Should().Throw<Exception>().WithMessage("An input following the \"--content-length\" option was not found.\nThe \"--content-length\" option must be followed by a valid integer");
    }
    
    [Fact]
    public void ParseContentLength_ShouldThrowExceptionWhenInputIsNotAnInteger()
    {
        var args = new List<string> { "--content-length", "two" };

        Action act = () => ParseContentLength(args);

        act.Should().Throw<Exception>().WithMessage("Error parsing --content-length input. Should be in the format of \"--content-length 5000\"");
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
    public void ParseTimeFormat_ShouldThrowExceptionWhenMoreThanOneTimeFlagIsPresent([CombinatorialValues("--ms", "--s", "--m", "--h")] string flag1, [CombinatorialValues("--ms", "--s", "--m", "--h")] string flag2)
    {
        var args = new List<string> { flag1, flag2 };

        Action act = () => ParseTimeFormat(args);

        act.Should().Throw<Exception>().WithMessage("Only one time format flag can be used at a time");
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
    
    #region ParseArgs
    
    [Fact]
    public void ParseArgs_ShouldReturnDefaultValuesWhenNoArgsArePassed()
    {
        var args = Array.Empty<string>();

        var actual = ParseArgs(args);

        actual.Should().BeEquivalentTo(new Arguments{Delay = 5000, Runtime = 0, Help = false, Debug = false});
    }
    
    [Fact]
    public void ParseArgs_ShouldReturnValuesWhenArgsArePassed()
    {
        var args = new[] { "--delay", "2000", "--content-length", "3000", "--help", "--debug" };

        var actual = ParseArgs(args);

        actual.Should().BeEquivalentTo(new Arguments{Delay = 2000, Runtime = 1000, Help = true, Debug = true});
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

        actual.Should().BeEquivalentTo(new Arguments{Delay = delayResult, Runtime = runtimeResult, Help = false});
    }
    
    [Fact]
    public void ParseArgs_ShouldThrowExceptionWhenContentLengthIsLessThanDelay()
    {
        var args = new[] { "--delay", "3000", "--content-length", "2000" };

        Action act = () => ParseArgs(args);

        act.Should().Throw<Exception>().WithMessage("Content length must be greater or equal to than the delay.");
    }
    
    #endregion
    
}