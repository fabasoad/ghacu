using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace Ghacu.GitHub
{
  public class GitHubClient : IGitHubClient
  {
    private readonly Octokit.IGitHubClient _client;

    internal GitHubClient(Octokit.IGitHubClient client)
    {
      _client = client;
    }

    public GitHubClient(string appName, string token)
    {
      var client = new Octokit.GitHubClient(new ProductHeaderValue(appName));
      if (token != null)
      {
        client.Credentials = new Credentials(token);
      }

      _client = client;
    }

    public async Task<string> GetLatestReleaseVersionAsync(string owner, string name)
    {
      Release result = await _client.Repository.Release.GetLatest(owner, name);
      return result.TagName;
    }

    public async Task<string> GetLatestTagVersionAsync(string owner, string name)
    {
      IReadOnlyList<RepositoryTag> allTags = await _client.Repository.GetAllTags(owner, name);
      return allTags?.LastOrDefault()?.Name;
    }
  }
}