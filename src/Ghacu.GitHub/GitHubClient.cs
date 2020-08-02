using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace Ghacu.GitHub
{
  public class GitHubClient : IGitHubClient
  {
    internal GitHubClient(Octokit.IGitHubClient client)
    {
      OctokitClient = client;
    }
    
    internal Octokit.IGitHubClient OctokitClient { get; }

    public GitHubClient(string appName, string token)
    {
      var client = new Octokit.GitHubClient(new ProductHeaderValue(appName));
      if (token != null)
      {
        client.Credentials = new Credentials(token);
      }

      OctokitClient = client;
    }

    public async Task<string> GetLatestReleaseVersionAsync(string owner, string name)
    {
      Release result = await OctokitClient.Repository.Release.GetLatest(owner, name);
      return result.TagName;
    }

    public async Task<string> GetLatestTagVersionAsync(string owner, string name)
    {
      IReadOnlyList<RepositoryTag> allTags = await OctokitClient.Repository.GetAllTags(owner, name);
      return allTags?.LastOrDefault()?.Name;
    }
  }
}