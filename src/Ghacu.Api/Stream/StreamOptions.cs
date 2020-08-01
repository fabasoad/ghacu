using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Ghacu.Api.Stream
{
  public class StreamOptions
  {
    public Exception Exception { get; set; }
    public LogLevel Level { get; set; }
    public IEnumerable<StreamMessage> Messages { get; set; }
  }
}