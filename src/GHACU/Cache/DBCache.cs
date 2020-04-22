using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GHACU.Workflow.Entities;
using LiteDB;

namespace GHACU.Cache
{
  internal sealed class DBCache
  {
    private const string DB_NAME = "e6DF9AfAmX1Sy7zHCX07VPHS";
    private const string ACTIONS_COLLECTION = "actions";
    private TimeSpan _storageTime = TimeSpan.FromMinutes(1);
    private Func<IRepositoryAware, Task<string>> _releaseRetriever;
    private IDictionary<IRepositoryAware, string> _localCache;
    internal DBCache(Func<IRepositoryAware, Task<string>> releaseRetriever)
    {
      _releaseRetriever = releaseRetriever;
      _localCache = new Dictionary<IRepositoryAware, string>();
    }

    internal async Task<string> Get(IRepositoryAware repositoryAware)
    {
      if (!_localCache.ContainsKey(repositoryAware))
      {
        _localCache.Add(repositoryAware, await GetFromDb(repositoryAware));
      }

      return _localCache[repositoryAware];
    }

    private async Task<string> GetFromDb(IRepositoryAware repositoryAware)
    {
      using (var db = new LiteDatabase(GetDbFilePath()))
      {
        var actionName = repositoryAware.FullName;
        var actions = db.GetCollection<DBAction>(ACTIONS_COLLECTION);
        var dbAction = actions.FindById(actionName);
        if (dbAction == null)
        {
          dbAction = new DBAction();
          dbAction.Name = actionName;
          dbAction.Version = await _releaseRetriever(repositoryAware);
          dbAction.Timestamp = DateTime.Now;
          actions.Insert(actionName, dbAction);
        }
        else if (DateTime.Now.Subtract(dbAction.Timestamp) > _storageTime)
        {
          dbAction.Version = await _releaseRetriever(repositoryAware);
          dbAction.Timestamp = DateTime.Now;
          actions.Update(actionName, dbAction);
        }

        return dbAction.Version;
      }
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