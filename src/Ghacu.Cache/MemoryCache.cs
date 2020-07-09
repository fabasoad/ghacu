using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ghacu.Api;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Telerik.JustMock")]
[assembly: InternalsVisibleTo("Ghacu.Cache.Tests")]
namespace Ghacu.Runner.Cache
{
  public class MemoryCache : ILatestVersionProvider
  {
    private readonly ILogger<MemoryCache> _logger;
    private readonly ILatestVersionProvider _provider;
    private readonly ISemaphore _semaphore;

    public MemoryCache(
      ILoggerFactory loggerFactory,
      Func<LatestVersionProviderType, ILatestVersionProvider> latestVersionProviderFactory,
      ISemaphore semaphore)
    {
      LocalCache = new ConcurrentDictionary<string, Task<string>>();
      _logger = loggerFactory.CreateLogger<MemoryCache>();
      _provider = latestVersionProviderFactory(LatestVersionProviderType.DbCache);
      _semaphore = semaphore;
    }

    internal IDictionary<string, Task<string>> LocalCache { get; }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      string key = $"{owner}/{repository}";
      if (LocalCache.ContainsKey(key))
      {
        _logger.LogInformation($"{owner}/{repository} latest release is retrieved from cache");
      }
      else
      {
        await _semaphore.WaitAsync();
        try
        {
          if (!LocalCache.ContainsKey(key))
          {
            LocalCache.Add(key, _provider.GetLatestVersionAsync(owner, repository));
          }
        }
        finally
        {
          _semaphore.Release();
        }
      }

      return await LocalCache[key];
    }
  }
}