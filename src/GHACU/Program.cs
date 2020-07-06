using System;
using Ghacu.Api;
using CommandLine;
using Ghacu.GitHub;
using Ghacu.Runner.Cache;
using Ghacu.Runner.Cli;
using Ghacu.Workflow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using StructureMap;

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
            .AddSingleton<IGlobalConfig, GlobalConfig>(_ => new GlobalConfig(o.GitHubToken, !o.NoCache))
            .AddTransient<GitHubClient>()
            .AddTransient<DbCache>()
            .AddTransient<MemoryCache>()
            .AddTransient<Func<LatestVersionProviderType, ILatestVersionProvider>>(serviceProvider => type =>
              type switch
              {
                LatestVersionProviderType.DB_CACHE => serviceProvider.GetService<DbCache>(),
                LatestVersionProviderType.MEMORY_CACHE => serviceProvider.GetService<MemoryCache>(),
                _ => serviceProvider.GetService<GitHubClient>()
              })
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
              _.AssemblyContainingType<ILatestVersionProvider>();
              _.AssemblyContainingType<IGitHubService>();
              _.WithDefaultConventions();
            });
            config.Populate(services);
          });
          container.GetInstance<IServiceProvider>().GetService<ICliService>().Run(o.Repository, o.Upgrade);
        });
    }
  }
}