using System;
using System.Threading.Tasks;
using GHACU.Workflow.Entities;

namespace GHACU.Cache
{
  public sealed class VersionsCache
  {
    private readonly DBCache _dbCache;
    private readonly Func<IRepositoryAware, Task<string>> _releaseRetriever;
    public VersionsCache(Func<IRepositoryAware, Task<string>> releaseRetriever)
    {
      _releaseRetriever = releaseRetriever;
      if (Program.UseCache)
      {
        _dbCache = new DBCache(releaseRetriever);
      }
    }

    public Task<string> Get(IRepositoryAware repositoryAware) =>
      _dbCache == null ? _releaseRetriever(repositoryAware) : _dbCache.Get(repositoryAware);
  }
}