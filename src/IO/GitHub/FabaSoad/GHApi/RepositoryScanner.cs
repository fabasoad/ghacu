using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace IO.GitHub.FabaSoad.GHApi
{
  public class RepositoryScanner
  {
    private const string APP_NAME = "ghacu";
    private GitHubClient _client;
    private IDictionary<string, Release> _cache = new Dictionary<string, Release>();
    public RepositoryScanner()
    {
      _client = new GitHubClient(new ProductHeaderValue(APP_NAME));
    }
    public async Task<string> GetLatestRelease(IRepositoryAware repositoryAware)
    {
      if (!_cache.ContainsKey(repositoryAware.FullName))
      {
        var release = await _client.Repository.Release.GetLatest(repositoryAware.Owner, repositoryAware.Name);        
        _cache.Add(repositoryAware.FullName, release);
      }
      return _cache[repositoryAware.FullName].TagName;
    }
  }
}