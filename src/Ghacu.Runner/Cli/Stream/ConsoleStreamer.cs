using System;

namespace Ghacu.Runner.Cli.Stream
{
  public class ConsoleStreamer : IStreamer
  {
    private readonly ConsoleColor _defaultColor;

    public ConsoleStreamer(ConsoleColor defaultColor)
    {
      _defaultColor = defaultColor;
    }

    public void Push(string format, params object[] args) => PushInternal(Console.Write, _defaultColor, format, args);

    public void Push(ConsoleColor color, string format, params object[] args) =>
      PushInternal(Console.Write, color, format, args);

    public void PushLine(string format, params object[] args) =>
      PushInternal(Console.WriteLine, _defaultColor, format, args);

    public void PushLine(ConsoleColor color, string format, params object[] args) =>
      PushInternal(Console.WriteLine, color, format, args);

    public void PushEmpty() => Console.WriteLine();

    private void PushInternal(Action<string, object[]> push, ConsoleColor color, string format, params object[] args)
    {
      Console.ForegroundColor = color;
      push(format, args);
      Console.ForegroundColor = _defaultColor;
    }
  }
}