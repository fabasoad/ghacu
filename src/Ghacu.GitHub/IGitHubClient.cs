using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace Ghacu.GitHub
{
  public interface IGitHubClient
  {
    Task<string> GetLatestReleaseVersionAsync(string owner, string name);
    Task<string> GetLatestTagVersionAsync(string owner, string name);
  }
}