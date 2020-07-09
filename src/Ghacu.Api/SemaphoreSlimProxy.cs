using System.Threading;
using System.Threading.Tasks;

namespace Ghacu.Api
{
  public class SemaphoreSlimProxy : ISemaphore
  {
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public Task WaitAsync() => _semaphore.WaitAsync();

    public int Release() => _semaphore.Release();
  }
}