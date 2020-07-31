using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.Api.Entities;
using Ghacu.Api.Version;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using GitHubAction = Ghacu.Api.Entities.Action;

namespace Ghacu.GitHub
{
  public class GitHubService : IGitHubService
  {
    private readonly ILogger<GitHubService> _logger;
    private readonly ILatestVersionProvider _provider;
    private readonly ISemaphoreSlimProxy _semaphore;

    public GitHubService(
      ILoggerFactory loggerFactory,
      ILatestVersionProvider versionProvider,
      ISemaphoreSlimProxy semaphore)
    {
      _logger = loggerFactory.CreateLogger<GitHubService>();
      _provider = versionProvider;
      _semaphore = semaphore;
    }

    public event Action<RepositoryCheckedArgs> RepositoryChecked;
    public event System.Action RepositoryCheckedFinished;
    public event Action<int> RepositoryCheckedStarted;

    public IDictionary<WorkflowInfo, IEnumerable<GitHubAction>> GetOutdated(IEnumerable<WorkflowInfo> items)
    {
      int totalCount = items
        .SelectMany(wfi => wfi.Workflow.Jobs)
        .SelectMany(j => j.Value.Steps)
        .Count(s => s.Action.IsValidForUpgrade);
      
      OnRepositoryCheckedStarted(totalCount);
      var index = 0;
      Dictionary<WorkflowInfo, IEnumerable<GitHubAction>> result = items
        .AsParallel()
        .ToDictionary(
          wfi => wfi,
          wfi => wfi.Workflow.Jobs
            .SelectMany(job => job.Value.Steps)
            .AsParallel()
            .Select(step => step.Action)
            .Where(action => action.IsValidForUpgrade)
            .Select(async action =>
            {
              if (action.LatestVersion == null)
              {
                await _semaphore.WaitAsync();
                try
                {
                  if (action.LatestVersion == null)
                  {
                    OnRepositoryChecked(++index, totalCount);
                    action.LatestVersion = await _provider.GetLatestVersionAsync(action.Owner, action.Repository);
                  }
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
      OnRepositoryCheckedFinished();
      return result;
    }

    private void OnRepositoryChecked(int index, int totalCount) =>
      RepositoryChecked?.Invoke(new RepositoryCheckedArgs(index, totalCount));

    private void OnRepositoryCheckedStarted(int totalCount) => RepositoryCheckedStarted?.Invoke(totalCount);
    private void OnRepositoryCheckedFinished() => RepositoryCheckedFinished?.Invoke();
  }
}