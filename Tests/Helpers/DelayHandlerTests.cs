using AudioDelay.Helpers;
using FluentAssertions;
using NSubstitute;
using Serilog;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;

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
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.InMemory()
            .CreateLogger();

        _delayHandler.Wait(3000, true);

        InMemorySink.Instance.Should().HaveMessage("Waiting for 3000 milliseconds...");
        InMemorySink.Instance.Should().HaveMessage("Waited for 1 seconds...");
        InMemorySink.Instance.Should().HaveMessage("Waited for 2 seconds...");
        InMemorySink.Instance.Should().HaveMessage("Waited for 3 seconds...");
        InMemorySink.Instance.Should().HaveMessage("Done waiting.");

        Log.CloseAndFlush();
    }

    [Fact]
    public void Wait_InvokesCallbackAfterDelayCompletes()
    {
        var callbackCalled = false;

        _delayHandler.Wait(1000, false, () => callbackCalled = true);

        callbackCalled.Should().BeTrue();
        _threadHandler.Received().Sleep(1000);
    }
}