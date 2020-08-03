using Ghacu.Api.Stream;
using Ghacu.Runner.Cli.Stream;
using Xunit;

namespace Ghacu.Runner.Tests.Cli.Stream
{
  public class SilentStreamerTest
  {
    [Fact]
    public void Push_Successful() => new SilentStreamer().Push<int>(new StreamOptions());
    
    [Fact]
    public void PushLine_Successful() => new SilentStreamer().PushLine<string>(new StreamOptions());
    
    [Fact]
    public void PushEmpty_Successful() => new SilentStreamer().PushEmpty();
    
    [Fact]
    public void Clear_Successful() => new SilentStreamer().Clear(1 /* any int */);
  }
}