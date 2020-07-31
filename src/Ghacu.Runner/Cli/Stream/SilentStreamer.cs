using Ghacu.Api.Stream;

namespace Ghacu.Runner.Cli.Stream
{
  public class SilentStreamer : IStreamer
  {
    public void Push<T>(StreamOptions options)
    {
    }

    public void PushLine<T>(StreamOptions options)
    {
    }

    public void PushEmpty()
    {
    }
  }
}