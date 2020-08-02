using System.Linq;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli.Stream
{
  public class LoggerStreamer : IStreamer
  {
    private readonly ILoggerFactory _loggerFactory;

    public LoggerStreamer(ILoggerFactory loggerFactory)
    {
      _loggerFactory = loggerFactory;
    }
    
    public void Push<T>(StreamOptions options) => _loggerFactory.CreateLogger<T>().Log(
      options.Level,
      options.Exception,
      string.Join(string.Empty, options.Messages.Select(m => m.Message)));
    
    public void PushLine<T>(StreamOptions options) => _loggerFactory.CreateLogger<T>().Log(
      options.Level,
      options.Exception,
      string.Join(string.Empty, options.Messages.Select(m => m.Message)));

    public void PushEmpty() { }
    
    public void Clear(int numLines)
    {
    }
  }
}