using System.Threading.Tasks;

namespace Ghacu.Api
{
  public interface ILatestVersionProvider
  {
    Task<string> GetLatestVersionAsync(string owner, string repository);
  }
}