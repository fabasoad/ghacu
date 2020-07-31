using System;

namespace Ghacu.Runner.Cli.Stream
{
  public interface IStreamer
  {
    void Push(string format, params object[] args);
    void Push(ConsoleColor color, string format, params object[] args);
    void PushLine(string format, params object[] args);
    void PushLine(ConsoleColor color, string format, params object[] args);
    void PushEmpty();
  }
}