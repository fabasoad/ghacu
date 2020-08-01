using System;
using System.Collections.Generic;

namespace Ghacu.Api.Stream
{
  public class StreamMessageBuilder
  {
    private readonly IList<StreamMessage> _messages = new List<StreamMessage>();

    public StreamMessageBuilder Add(string message, ConsoleColor? color = null)
    {
      _messages.Add(new StreamMessage { Color = color, Message = message });
      return this;
    }

    public IEnumerable<StreamMessage> Build() => _messages;
  }
}