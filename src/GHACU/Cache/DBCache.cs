using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GHACU.Workflow.Entities;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace GHACU.Cache
{
  internal sealed class DBCache
  {
    private const string DB_NAME = "e6DF9AfAmX1Sy7zHCX07VPHS";
    private const string ACTIONS_COLLECTION = "actions";
    private readonly TimeSpan _storageTime = TimeSpan.FromMinutes(1);
    private readonly Func<IRepositoryAware, Task<string>> _releaseRetriever;
    private readonly IDictionary<string, Task<string>> _localCache;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly ILogger<DBCache> _logger;

    internal DBCache(Func<IRepositoryAware, Task<string>> releaseRetriever)
    {
      _releaseRetriever = releaseRetriever;
      _localCache = new ConcurrentDictionary<string, Task<string>>();
      _logger = Program.LoggerFactory.CreateLogger<DBCache>();
    }

    internal async Task<string> Get(IRepositoryAware repositoryAware)
    {
      var key = repositoryAware.FullName;
      if (_localCache.ContainsKey(key))
      {
        _logger.LogInformation($"{repositoryAware.FullName} latest release is retrieved from cache");
      }
      else
      {
        await _semaphore.WaitAsync();
        try
        {
          if (!_localCache.ContainsKey(key))
          {
            _localCache.Add(key, GetFromDb(repositoryAware));
          }
        }
        finally
        {
          _semaphore.Release();
        }
      }

      return await _localCache[key];
    }

    private async Task<string> GetFromDb(IRepositoryAware repositoryAware)
    {
      using var db = new LiteDatabase(GetDbFilePath());
      var actionName = repositoryAware.FullName;
      var actions = db.GetCollection<DBAction>(ACTIONS_COLLECTION);
      DBAction dbAction = actions.FindById(actionName);
      if (dbAction == null)
      {
        dbAction = new DBAction
        {
          Name = actionName,
          Version = await _releaseRetriever(repositoryAware),
          Timestamp = DateTime.Now
        };
        actions.Insert(actionName, dbAction);
      }
      else if (DateTime.Now.Subtract(dbAction.Timestamp) > _storageTime)
      {
        dbAction.Version = await _releaseRetriever(repositoryAware);
        dbAction.Timestamp = DateTime.Now;
        actions.Update(actionName, dbAction);
      }
      else
      {
        _logger.LogInformation($"{repositoryAware.FullName} version is retrieved from local DB");
      }

      return dbAction.Version;
    }

    private string GetDbFilePath()
    {
      var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      var ghacuFolder = Path.Combine(appData, "ghacu");
      if (!Directory.Exists(ghacuFolder))
      {
        Directory.CreateDirectory(ghacuFolder);
      }

      return Path.Combine(ghacuFolder, DB_NAME);
    }
  }
}