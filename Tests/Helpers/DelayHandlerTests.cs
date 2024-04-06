using AudioDelay.Helpers;
using FluentAssertions;
using NSubstitute;

namespace Tests.Helpers;

public class DelayHandlerTests
{
    private readonly IThreadHandler _threadHandler;
    private readonly DelayHandler _delayHandler;
    
    public DelayHandlerTests()
    {
        _threadHandler = Substitute.For<IThreadHandler>();
        _delayHandler = new DelayHandler(_threadHandler);
    }

    [Fact]
    public void Wait_WaitsForSpecifiedTime()
    {
        _delayHandler.Wait(1000, false);

        _threadHandler.Received().Sleep(1000);
    }
    
    [Fact]
    public void Wait_Debug_WaitsForSpecifiedTimeInChunks()
    {
        _delayHandler.Wait(3000, true);

        _threadHandler.Received().Sleep(1000);
        _threadHandler.Received().Sleep(1000);
        _threadHandler.Received().Sleep(1000);
    }
    
    [Fact]
    public void Wait_Debug_WaitsForSpecifiedTimeInChunksWithRemainder()
    {
        _delayHandler.Wait(3001, true);

        _threadHandler.Received().Sleep(1000);
        _threadHandler.Received().Sleep(1000);
        _threadHandler.Received().Sleep(1000);
        _threadHandler.Received().Sleep(1);
    }

    [Fact]
    public void Wait_Debug_LogsCorrectly()
    {
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        _delayHandler.Wait(3000, true);
        
        var output = stringWriter.ToString().Split(Environment.NewLine);

        output.Should().Contain("Waiting for 3000 milliseconds...");
        output.Should().Contain("Waited for 1 seconds...");
        output.Should().Contain("Waited for 2 seconds...");
        output.Should().Contain("Waited for 3 seconds...");
        output.Should().Contain("Done waiting.");
        
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        Console.SetOut(standardOutput);
    }
}