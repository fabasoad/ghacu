using System;
using System.Linq;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli.Stream
{
  public class ConsoleStreamer : IConsoleStreamer
  {
    private readonly LogLevel _logLevel;
    private readonly ConsoleColor _defaultColor;

    public ConsoleStreamer(LogLevel logLevel, ConsoleColor defaultColor)
    {
      _logLevel = logLevel;
      _defaultColor = defaultColor;
    }

    public void Push<T>(StreamOptions options) => PushInternal(Console.Write, options);

    public void PushLine<T>(StreamOptions options) => PushInternal(Console.WriteLine, options);

    public void PushEmpty() => Console.WriteLine();

    private void PushInternal(Action<string> push, StreamOptions options)
    {
      if (_logLevel.CompareTo(options.Level) > 0)
      {
        return;
      }

      int totalCount = options.Messages.Count();
      for (var i = 0; i < totalCount; i++)
      {
        StreamMessage streamMessage = options.Messages.ElementAt(i);
        Console.ForegroundColor = streamMessage.Color ?? _defaultColor;
        if (i == totalCount - 1)
        {
          push(streamMessage.Message);
        }
        else
        {
          Console.Write(streamMessage.Message);
        }

        Console.ForegroundColor = _defaultColor;
      }
    }
  }
}