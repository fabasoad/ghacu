using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ghacu.Api;
using Microsoft.Extensions.Logging;

namespace GHACU.Cache
{
  public class MemoryCache : ILatestVersionProvider
  {
    private readonly IDictionary<string, Task<string>> _localCache;
    private readonly ILogger<MemoryCache> _logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private ILatestVersionProvider _provider;

    public MemoryCache(ILoggerFactory loggerFactory)
    {
      _localCache = new ConcurrentDictionary<string, Task<string>>();
      _logger = loggerFactory.CreateLogger<MemoryCache>();
    }

    public async Task<string> GetLatestVersion(string owner, string repository)
    {
      var key = $"{owner}/{repository}";
      if (_localCache.ContainsKey(key))
      {
        _logger.LogInformation($"{owner}/{repository} latest release is retrieved from cache");
      }
      else
      {
        await _semaphore.WaitAsync();
        try
        {
          if (!_localCache.ContainsKey(key))
          {
            _localCache.Add(key, _provider.GetLatestVersion(owner, repository));
          }
        }
        finally
        {
          _semaphore.Release();
        }
      }

      return await _localCache[key];
    }
  }
}