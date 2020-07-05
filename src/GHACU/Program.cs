using System;
using CommandLine;
using GHACU.CLI;
using GHACU.Workflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StructureMap;

namespace GHACU
{
  public class Program
  {
    public static bool UseCache { get; private set; }

    public static void Main(string[] args)
    {
      Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(o =>
        {
          IServiceCollection services = new ServiceCollection()
            .AddLogging(b => b
              .AddConsole(options =>
              {
                options.DisableColors = true;
                options.Format = ConsoleLoggerFormat.Default;
              })
              .SetMinimumLevel(o.LogLevel));

          var container = new Container();
          container.Configure(config =>
          {
            config.Scan(_ =>
            {
              _.AssemblyContainingType<Program>();
              _.AssemblyContainingType<WorkflowService>();
              _.WithDefaultConventions();
            });
            config.Populate(services);
          });
          UseCache = !o.NoCache;
          container.GetInstance<IServiceProvider>().GetService<ICliService>().Run(o);
        });
    }
  }
}