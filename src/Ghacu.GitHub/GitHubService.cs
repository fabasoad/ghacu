using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api;
using Ghacu.Api.Entities;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Action = Ghacu.Api.Entities.Action;

namespace Ghacu.GitHub
{
  public class GitHubService : IGitHubService
  {
    private readonly ILogger<GitHubService> _logger;
    private readonly ILatestVersionProvider _provider;
    private readonly ISemaphoreSlimProxy _semaphore;

    public GitHubService(
      ILoggerFactory loggerFactory,
      Func<LatestVersionProviderType, ILatestVersionProvider> latestVersionProviderFactory,
      IGlobalConfig globalConfig,
      ISemaphoreSlimProxy semaphore)
    {
      _logger = loggerFactory.CreateLogger<GitHubService>();
      _provider = latestVersionProviderFactory(
        globalConfig.UseCache ? LatestVersionProviderType.MemoryCache : LatestVersionProviderType.GitHub);
      _semaphore = semaphore;
    }

    public IDictionary<WorkflowInfo, IEnumerable<Action>> GetOutdated(IEnumerable<WorkflowInfo> items)
    {
      return items
        .AsParallel()
        .ToDictionary(
          wfi => wfi,
          wfi => wfi.Workflow.Jobs
            .SelectMany(job => job.Value.Steps)
            .AsParallel()
            .Select(step => step.Action)
            .Where(action => action.Type != UsesType.Internal)
            .Select(async action =>
            {
              if (action.LatestVersion == null)
              {
                await _semaphore.WaitAsync();
                try
                {
                  action.LatestVersion = await _provider
                    .GetLatestVersionAsync(action.Owner, action.ActionName);
                }
                catch (GitHubVersionNotFoundException e)
                {
                  _logger.LogWarning(e, e.Message);
                }
                finally
                {
                  _semaphore.Release();
                }
              }

              return action;
            })
            .Select(t => t.Result)
            .Where(action => !action.IsUpToDate))
        .Where(p => p.Value.Any())
        .ToDictionary(p => p.Key, p => p.Value.AsEnumerable());
    }
  }
}