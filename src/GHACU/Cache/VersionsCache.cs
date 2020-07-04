using System;
using System.Threading.Tasks;
using GHACU.Workflow.Entities;

namespace GHACU.Cache
{
  public sealed class VersionsCache
  {
    private DBCache _dbCache;
    public VersionsCache(Func<IRepositoryAware, Task<string>> releaseRetriever)
    {
      _dbCache = new DBCache(releaseRetriever);
    }

    public Task<string> Get(IRepositoryAware repositoryAware) => _dbCache.Get(repositoryAware);
  }
}