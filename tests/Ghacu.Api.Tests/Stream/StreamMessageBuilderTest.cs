using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Stream;
using MoreLinq.Extensions;
using Xunit;

namespace Ghacu.Api.Tests.Stream
{
  public class StreamMessageBuilderTest
  {
    [Fact]
    public void Build_Correctly()
    {
      var builder = new StreamMessageBuilder();
      IEnumerable<StreamMessage> result = builder
        .Add("test-message-1", (ConsoleColor)1)
        .Add("test-message-2", (ConsoleColor)2)
        .Add("test-message-3")
        .Build();
      Assert.NotNull(result);
      Assert.Equal(3, result.Count());
      result.ForEach((m, i) =>
      {
        if (i == 2)
        {
          Assert.Null(m.Color);
        }
        else
        {
          Assert.Equal((ConsoleColor)(i + 1), m.Color);
        }

        Assert.Equal($"test-message-{i + 1}", m.Message);
      });
    }
  }
}