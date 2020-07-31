using System;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli.Stream
{
  public class ConsoleStreamer : IStreamer
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
      if (_logLevel.CompareTo(options.Level) <= 0)
      {
        Console.ForegroundColor = options.Color ?? _defaultColor;
        push(options.Message);
        Console.ForegroundColor = _defaultColor;
      }
    }
  }
}