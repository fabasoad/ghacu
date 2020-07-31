using System;
using Microsoft.Extensions.Logging;

namespace Ghacu.Api.Stream
{
  public class StreamOptions
  {
    public ConsoleColor? Color { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
  }
}