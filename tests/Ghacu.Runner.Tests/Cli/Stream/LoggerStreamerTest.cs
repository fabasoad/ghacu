using System;
using System.Collections.Generic;
using Ghacu.Api.Stream;
using Ghacu.Runner.Cli.Stream;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ghacu.Runner.Tests.Cli.Stream
{
  public class LoggerStreamerTest
  {
    [Fact]
    public void Push_Successful() =>
      RunSuccessfulTest((streamer, options) => streamer.Push<LoggerStreamerTest>(options));
    
    [Fact]
    public void PushLine_Successful() =>
      RunSuccessfulTest((streamer, options) => streamer.PushLine<LoggerStreamerTest>(options));

    private static void RunSuccessfulTest(Action<IStreamer, StreamOptions> runner)
    {
      const string message1 = "test-";
      const string message2 = "message";
      const LogLevel logLevel = LogLevel.Debug;
      var expectedException = new Exception("test-exception-message");
      var stubLogger = new StubLogger(logLevel, expectedException, message1 + message2);
      var streamer = new LoggerStreamer(new StubLoggerFactory(stubLogger));
      runner(streamer, new StreamOptions
      {
        Level = logLevel,
        Exception = expectedException,
        Messages = new List<StreamMessage>
        {
          new StreamMessage { Message = message1 },
          new StreamMessage { Message = message2 }
        }
      });
      Assert.Equal(1, stubLogger.NumCalls);
    }

    [Fact]
    public void PushEmpty_Successful() => new LoggerStreamer(null).PushEmpty();

    [Fact]
    public void Clear_Successful() => new LoggerStreamer(null).Clear(3 /* any int */);
  }

  internal class StubLoggerFactory : ILoggerFactory
  {
    private readonly ILogger _logger;

    internal StubLoggerFactory(ILogger logger)
    {
      _logger = logger;
    }
    
    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName) => _logger;

    public void AddProvider(ILoggerProvider provider)
    {
    }
  }

  internal class StubLogger : ILogger
  {
    private readonly LogLevel _expectedLogLevel;
    private readonly Exception _expectedException;
    private readonly string _expectedMessage;

    internal StubLogger(LogLevel expectedLogLevel, Exception expectedException, string expectedMessage)
    {
      _expectedLogLevel = expectedLogLevel;
      _expectedException = expectedException;
      _expectedMessage = expectedMessage;
    }

    internal int NumCalls { get; private set; }

    public void Log<TState>(
      LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      NumCalls++;
      Assert.Equal(_expectedLogLevel, logLevel);
      Assert.Equal(_expectedException, exception);
      Assert.Equal(_expectedMessage, formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => null;
  }
}