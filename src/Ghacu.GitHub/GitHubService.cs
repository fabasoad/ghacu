using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Ghacu.Api;
using Ghacu.Api.Entities;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ghacu.GitHub
{
  public class GitHubService : IGitHubService
  {
    private readonly ILatestVersionProvider _provider;
    private readonly ILogger<GitHubService> _logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public GitHubService(ILoggerFactory loggerFactory, Func<LatestVersionProviderType, ILatestVersionProvider> latestVersionProviderFactory)
    {
      _logger = loggerFactory.CreateLogger<GitHubService>();
      _provider = latestVersionProviderFactory(LatestVersionProviderType.MEMORY_CACHE);
    }

    public IDictionary<WorkflowInfo, IEnumerable<Step>> GetOutdated(IEnumerable<WorkflowInfo> items)
    {
      return items
        .AsParallel()
        .ToDictionary(
          wfi => wfi,
          wfi => wfi.Workflow.Jobs
            .SelectMany(job => job.Value.Steps)
            .AsParallel()
            .Where(step => !step.IsInternal)
            .Select(async step =>
            {
              if (step.Uses.GetLatestVersion() == null)
              {
                await _semaphore.WaitAsync();
                try
                {
                  string latestVersion = await _provider
                    .GetLatestVersionAsync(step.Uses.Owner, step.Uses.ActionName);
                  step.Uses.SetLatestVersion(latestVersion);
                }
                catch (GitHubVersionNotFoundException e)
                {
                  _logger.LogError(e, e.Message);
                }
                finally
                {
                  _semaphore.Release();
                }
              }

              return step;
            })
            .Select(t => t.Result)
            .Where(step => !step.IsUpToDate))
        .Where(p => p.Value.Any())
        .ToDictionary(p => p.Key, p => p.Value.AsEnumerable());
    }
  }
}