using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api;
using Ghacu.Api.Entities;
using Ghacu.Api.Stream;
using Ghacu.Api.Version;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using GitHubAction = Ghacu.Api.Entities.Action;

namespace Ghacu.GitHub
{
  public class GitHubService : IGitHubService
  {
    private readonly ILatestVersionProvider _provider;
    private readonly ISemaphoreSlimProxy _semaphore;
    private readonly IStreamer _streamer;

    public GitHubService(
      ILatestVersionProvider versionProvider,
      ISemaphoreSlimProxy semaphore,
      IStreamer streamer)
    {
      _provider = versionProvider;
      _semaphore = semaphore;
      _streamer = streamer;
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
                  _streamer.PushLine<GitHubService>(
                    new StreamOptions { Level = LogLevel.Warning, Message = e.Message });
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