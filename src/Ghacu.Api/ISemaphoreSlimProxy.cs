using System.Threading.Tasks;

namespace Ghacu.Api
{
  public interface ISemaphoreSlimProxy
  {
    Task WaitAsync();
    int Release();
  }
}