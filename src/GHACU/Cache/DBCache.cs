using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GHACU.PubSub;
using GHACU.Workflow.Entities;
using LiteDB;
using PubSub;

namespace GHACU.Cache
{
  internal sealed class DBCache
  {
    private const string DB_NAME = "e6DF9AfAmX1Sy7zHCX07VPHS";
    private const string ACTIONS_COLLECTION = "actions";
    private readonly TimeSpan _storageTime = TimeSpan.FromMinutes(1);
    private readonly Func<IRepositoryAware, Task<string>> _releaseRetriever;
    private readonly IDictionary<IRepositoryAware, Task<string>> _localCache;
    private readonly Hub _hub;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    internal DBCache(Func<IRepositoryAware, Task<string>> releaseRetriever)
    {
      _hub = Hub.Default;
      _releaseRetriever = releaseRetriever;
      _localCache = new ConcurrentDictionary<IRepositoryAware, Task<string>>();
    }

    internal Task<string> Get(IRepositoryAware repositoryAware)
    {
      if (_localCache.ContainsKey(repositoryAware))
      {
        _hub.Publish(new PubSubMessage
        {
          Action = PubSubAction.SUCCEED,
          Topic = PubSubTopic.DB,
          Message =
            $"{repositoryAware.FullName} latest release = {_localCache[repositoryAware]} is retrieved from cache"
        });
      }
      else
      {
        _localCache.Add(repositoryAware, GetFromDb(repositoryAware));
      }

      return _localCache[repositoryAware];
    }

    private async Task<string> GetFromDb(IRepositoryAware repositoryAware)
    {
      DBAction dbAction;
      await _semaphore.WaitAsync();
      using var db = new LiteDatabase(GetDbFilePath());
      var actionName = repositoryAware.FullName;
      try
      {
        var actions = db.GetCollection<DBAction>(ACTIONS_COLLECTION);
        dbAction = actions.FindById(actionName);
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
          _hub.Publish(new PubSubMessage
          {
            Action = PubSubAction.SUCCEED,
            Topic = PubSubTopic.DB,
            Message = $"{repositoryAware.FullName} version is retrieved from local DB"
          });
        }
      }
      finally
      {
        _semaphore.Release();
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