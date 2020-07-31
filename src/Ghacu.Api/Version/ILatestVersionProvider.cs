using System.Threading.Tasks;

namespace Ghacu.Api.Version
{
  public interface ILatestVersionProvider
  {
    Task<string> GetLatestVersionAsync(string owner, string repository);
  }
}