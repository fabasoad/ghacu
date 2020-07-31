using System;
using System.Runtime.CompilerServices;
using CommandLine;
using Ghacu.Api;
using Ghacu.Cache;
using Ghacu.GitHub;
using Ghacu.Runner.Cli;
using Ghacu.Runner.Cli.Print;
using Ghacu.Runner.Cli.Progress;
using Ghacu.Runner.Cli.Stream;
using Ghacu.Workflow;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StructureMap;

[assembly: InternalsVisibleTo("Ghacu.Runner.Tests")]

namespace Ghacu.Runner
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class Program
  {
    private const string APP_NAME = "ghacu";
    private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";

    public static void Main(string[] args)
    {
      Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed(o =>
        {
          IServiceCollection services = new ServiceCollection();
          if (o.OutputType == OutputType.Silent)
          {
            services.AddTransient<IStreamer, SilentStreamer>();
          }
          else
          {
            services.AddTransient<IStreamer, ConsoleStreamer>(_ => new ConsoleStreamer(Console.ForegroundColor));
          }

          services
            .AddSingleton<IGlobalConfig, GlobalConfig>(_ => new GlobalConfig(o.UseCache == BooleanOption.Yes))
            .AddTransient(serviceProvider =>
            {
              if (o.OutputType == OutputType.Silent || o.LogLevel.CompareTo(LogLevel.Error) < 0)
              {
                return _ => new SilentProgressBar();
              }

              var dict = new Func<int, IProgressBar>[]
              {
                totalTicks => new GhacuShellProgressBar(totalTicks),
                totalTicks => new PercentageProgressBar(totalTicks, serviceProvider.GetService<IStreamer>())
              };
              return dict[new Random().Next(0, dict.Length)];
            })
            .AddTransient<GitHubVersionProvider>()
            .AddTransient<DbCacheVersionProvider>()
            .AddTransient<MemoryCacheVersionProvider>()
            .AddTransient<Func<LatestVersionProviderType, ILatestVersionProvider>>(serviceProvider => type =>
              type switch
              {
                LatestVersionProviderType.DbCache => serviceProvider.GetService<DbCacheVersionProvider>(),
                LatestVersionProviderType.MemoryCache => serviceProvider.GetService<MemoryCacheVersionProvider>(),
                _ => serviceProvider.GetService<GitHubVersionProvider>()
              })
            .AddTransient<IGitHubClient, GitHubClient>(
              _ => new GitHubClient(APP_NAME, o.GitHubToken ?? Environment.GetEnvironmentVariable(ENV_GITHUB_TOKEN)))
            .AddTransient<Func<string, ILiteDatabase>>(_ => filePath => new LiteDatabase(filePath))
            .AddLogging(b => b
              .AddConsole(options =>
              {
                options.DisableColors = o.UseColors == BooleanOption.No;
                options.Format = ConsoleLoggerFormat.Default;
              })
              .SetMinimumLevel(o.LogLevel));

          if (o.UseColors == BooleanOption.Yes)
          {
            services.AddTransient<IActionPrinter, ColorActionPrinter>();
          }
          else
          {
            services.AddTransient<IActionPrinter, NoColorActionPrinter>();
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
              _.AssemblyContainingType<DbCacheVersionProvider>();
              _.WithDefaultConventions();
            });
            config.Populate(services);
          });
          container.GetInstance<IServiceProvider>().GetService<ICliService>().Run(o.Repository, o.Upgrade);
        });
    }
  }
}