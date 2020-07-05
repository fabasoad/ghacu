using System.Collections.Generic;
using System.Linq;
using Api.Entities;
using GHACU.Workflow;
using GHACU.Workflow.Entities;

namespace GHACU.GitHub
{
  public class GitHubService : IGitHubService
  {
    private readonly IGitHubClient _gitHubClient;

    public GitHubService(IGitHubClient gitHubClient)
    {
      _gitHubClient = gitHubClient;
    }

    public IDictionary<(string, IWorkflowFile), IEnumerable<IStep>> GetOutdated(string token, IEnumerable<IWorkflowInfo> items)
    {
      return items
        .AsParallel()
        .ToDictionary(
          wfi => (wfi.Workflow.Name, wfi.File),
          wfi => wfi.Workflow.Jobs
            .SelectMany(job => job.Value.Steps)
            .AsParallel()
            .Select(async step =>
            {
              if (step.Uses.LatestVersion == null)
              {
                step.Uses.UpdateLatestVersion(
                  await _gitHubClient.WithToken(token).GetLatestRelease(step.Uses.Owner, step.Uses.ActionName));
              }

              return step;
            })
            .Select(t => t.Result)
            .Where(step => !step.IsUpToDate)
            .AsSequential());
    }
  }
}