using System;
using Ghacu.Api.Stream;
using Xunit;

namespace Ghacu.Api.Tests.Stream
{
  public class StreamMessageTest
  {
    [Fact]
    public void Test_GettersSetters()
    {
      var streamMessage = new StreamMessage();
      Assert.Null(streamMessage.Color);
      Assert.Null(streamMessage.Message);
      
      const ConsoleColor color = ConsoleColor.Cyan;
      const string message = "test-message";
      streamMessage.Color = color;
      streamMessage.Message = message;
      Assert.Equal(color, streamMessage.Color);
      Assert.Equal(message, streamMessage.Message);
    }
  }
}