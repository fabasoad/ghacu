using System.Threading.Tasks;

namespace Ghacu.Api
{
  public interface ISemaphore
  {
    Task WaitAsync();
    int Release();
  }
}