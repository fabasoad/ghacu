using CommandLine;
using GHACU.CLI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using MicrosoftLoggerFactory = Microsoft.Extensions.Logging.LoggerFactory;

namespace GHACU
{
  public class Program
  {
    public static ILoggerFactory LoggerFactory { get; private set; }

    public static bool UseCache { get; private set; }

    public static void Main(string[] args)
    {
      Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(o =>
        {
          LoggerFactory = MicrosoftLoggerFactory
            .Create(b => b
              .AddConsole(options =>
              {
                options.DisableColors = true;
                options.Format = ConsoleLoggerFormat.Default;
              })
              .SetMinimumLevel(o.LogLevel));
          UseCache = !o.NoCache;
          new OptionsHandler().Handle(o);
        });
    }
  }
}