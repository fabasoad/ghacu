using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ghacu.Api.Tests.Stream
{
  public class StreamOptionsTest
  {
    [Fact]
    public void Test_GettersSetters()
    {
      const LogLevel level = LogLevel.Critical;
      var exception = new Exception("test-message");
      var streamMessage1 = new StreamMessage();
      var streamMessage2 = new StreamMessage();
      var messages = new List<StreamMessage> { streamMessage1, streamMessage2 };
      var options = new StreamOptions
      {
        Exception = exception,
        Level = level,
        Messages = messages
      };
      Assert.Same(exception, options.Exception);
      Assert.Equal(level, options.Level);
      Assert.NotNull(options.Messages);
      Assert.Equal(2, options.Messages.Count());
      Assert.Same(messages, options.Messages);
      Assert.Contains(streamMessage1, options.Messages);
      Assert.Contains(streamMessage2, options.Messages);
    }
  }
}