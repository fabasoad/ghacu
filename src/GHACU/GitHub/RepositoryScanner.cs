using System;
using System.Threading.Tasks;
using GHACU.Cache;
using GHACU.Workflow.Entities;
using Octokit;

namespace GHACU.GitHub
{
  public sealed class RepositoryScanner
  {
    private const string APP_NAME = "ghacu";
    private GitHubClient _client;
    private VersionsCache _cache;
    public RepositoryScanner()
    {
      _client = new GitHubClient(new ProductHeaderValue(APP_NAME));
      _cache = new VersionsCache(PullLatestVersion);
    }
    public async Task<string> GetLatestVersion(IRepositoryAware repositoryAware) =>
      await _cache.Get(repositoryAware);
    private async Task<string> PullLatestVersion(IRepositoryAware r)
    {
      try
      {
        return (await _client.Repository.Release.GetLatest(r.Owner, r.Name)).TagName;
      }
      catch (RateLimitExceededException e)
      {
        Console.WriteLine(e.Message);
        return "N/A";
      }
    }
  }
}