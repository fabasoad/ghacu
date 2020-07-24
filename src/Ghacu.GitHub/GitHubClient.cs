using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace Ghacu.GitHub
{
  public class GitHubClient : IGitHubClient
  {
    private readonly Octokit.GitHubClient _client;

    public GitHubClient(string appName, string token)
    {
      _client = new Octokit.GitHubClient(new ProductHeaderValue(appName));
      if (token != null)
      {
        _client.Credentials = new Credentials(token);
      }
    }
    
    public Task<string> GetLatestReleaseVersionAsync(string owner, string name) =>
      _client.Repository.Release.GetLatest(owner, name).ContinueWith(t => t.Result.TagName);

    public Task<string> GetLatestTagVersionAsync(string owner, string name) =>
      _client.Repository.GetAllTags(owner, name).ContinueWith(t => t.Result.LastOrDefault()?.Name);
  }
}