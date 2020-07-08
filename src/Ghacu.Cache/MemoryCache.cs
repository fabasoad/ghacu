using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ghacu.Api;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Telerik.JustMock")]
[assembly: InternalsVisibleTo("Ghacu.Cache.Tests")]

namespace Ghacu.Runner.Cache
{
  public class MemoryCache : ILatestVersionProvider
  {
    private readonly IDictionary<string, Task<string>> _localCache;
    private readonly ILogger<MemoryCache> _logger;
    private readonly ILatestVersionProvider _provider;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public MemoryCache(
      ILoggerFactory loggerFactory,
      Func<LatestVersionProviderType, ILatestVersionProvider> latestVersionProviderFactory)
    {
      _localCache = new ConcurrentDictionary<string, Task<string>>();
      _logger = loggerFactory.CreateLogger<MemoryCache>();
      _provider = latestVersionProviderFactory(LatestVersionProviderType.DbCache);
    }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      string key = $"{owner}/{repository}";
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
            _localCache.Add(key, _provider.GetLatestVersionAsync(owner, repository));
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