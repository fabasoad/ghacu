using System.Threading.Tasks;

namespace GHACU.GitHub
{
  public interface IGitHubClient
  {
    Task<string> GetLatestRelease(string owner, string repository);
    IGitHubClient WithToken(string token);
  }
}