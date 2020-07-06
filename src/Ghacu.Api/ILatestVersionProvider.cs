using System.Threading.Tasks;

namespace Ghacu.Api
{
  public interface ILatestVersionProvider
  {
    Task<string> GetLatestVersion(string owner, string repository);
  }
}