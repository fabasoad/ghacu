using System;
using System.Linq;
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
      Exception lastException;
      try
      {
        return (await _client.Repository.Release.GetLatest(r.Owner, r.Name)).TagName;
      }
      catch (NotFoundException)
      {
        try
        {
          return (await _client.Repository.GetAllTags(r.Owner, r.Name)).Last().Name;
        }
        catch (Exception e)
        {
          lastException = e;
        }
      }
      catch (Exception e)
      {
        lastException = e;
      }

      if (lastException != null)
      {
        Console.WriteLine(lastException.Message);
      }

      return "N/A";
    }
  }
}