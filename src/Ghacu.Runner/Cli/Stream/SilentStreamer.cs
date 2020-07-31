using System;

namespace Ghacu.Runner.Cli.Stream
{
  public class SilentStreamer : IStreamer
  {
    public void Push(string format, params object[] args)
    {
    }

    public void Push(ConsoleColor color, string format, params object[] args)
    {
    }

    public void PushLine(string format, params object[] args)
    {
    }

    public void PushLine(ConsoleColor color, string format, params object[] args)
    {
    }

    public void PushEmpty()
    {
    }
  }
}