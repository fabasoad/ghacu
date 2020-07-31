using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.Api.Stream;
using Ghacu.Api.Version;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Telerik.JustMock")]
[assembly: InternalsVisibleTo("Ghacu.Cache.Tests")]

namespace Ghacu.Cache
{
  public class MemoryCacheVersionProvider : IMemoryCacheVersionProvider
  {
    private readonly IDbCacheVersionProvider _provider;
    private readonly ISemaphoreSlimProxy _semaphore;
    private readonly IStreamer _streamer;

    public MemoryCacheVersionProvider(
      IDbCacheVersionProvider versionProvider,
      ISemaphoreSlimProxy semaphore,
      IStreamer streamer)
    {
      _provider = versionProvider;
      _semaphore = semaphore;
      _streamer = streamer;
      LocalCache = new ConcurrentDictionary<string, Task<string>>();
    }

    internal IDictionary<string, Task<string>> LocalCache { get; }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      string key = $"{owner}/{repository}";
      if (LocalCache.ContainsKey(key))
      {
        _streamer.PushLine<MemoryCacheVersionProvider>(new StreamOptions
        {
          Level = LogLevel.Debug,
          Message = $"{owner}/{repository} latest release is retrieved from cache"
        });
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