using System;
using System.Runtime.CompilerServices;
using CommandLine;
using Ghacu.Api;
using Ghacu.Cache;
using Ghacu.GitHub;
using Ghacu.Runner.Cli;
using Ghacu.Runner.Cli.Print;
using Ghacu.Workflow;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StructureMap;

[assembly: InternalsVisibleTo("Ghacu.Runner.Tests")]

namespace Ghacu.Runner
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(o =>
        {
          IServiceCollection services = new ServiceCollection()
            .AddSingleton<IGlobalConfig, GlobalConfig>(_ => new GlobalConfig(o.GitHubToken, o.UseCache == BooleanOption.Yes))
            .AddTransient<GitHubClient>()
            .AddTransient<DbCache>()
            .AddTransient<MemoryCache>()
            .AddTransient<Func<LatestVersionProviderType, ILatestVersionProvider>>(serviceProvider => type =>
              type switch
              {
                LatestVersionProviderType.DbCache => serviceProvider.GetService<DbCache>(),
                LatestVersionProviderType.MemoryCache => serviceProvider.GetService<MemoryCache>(),
                _ => serviceProvider.GetService<GitHubClient>()
              })
            .AddTransient<Func<string, ILiteDatabase>>(_ => filePath => new LiteDatabase(filePath))
            .AddLogging(b => b
              .AddConsole(options =>
              {
                options.DisableColors = true;
                options.Format = ConsoleLoggerFormat.Default;
              })
              .SetMinimumLevel(o.LogLevel));

          switch (o.OutputType)
          {
            case OutputType.Color:
              services.AddTransient<IActionPrinter, ColorActionPrinter>();
              break;
            case OutputType.NoColor:
              services.AddTransient<IActionPrinter, NoColorActionPrinter>();
              break;
          }

          using var container = new Container();
          container.Configure(config =>
          {
            config.Scan(_ =>
            {
              _.AssemblyContainingType<Program>();
              _.AssemblyContainingType<IWorkflowService>();
              _.AssemblyContainingType<ILatestVersionProvider>();
              _.AssemblyContainingType<IGitHubService>();
              _.AssemblyContainingType<DbCache>();
              _.WithDefaultConventions();
            });
            config.Populate(services);
          });
          container.GetInstance<IServiceProvider>().GetService<ICliService>().Run(o.Repository, o.Upgrade);
        });
    }
  }
}