using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GHACU.Workflow.Entities;
using Octokit;

namespace GHACU.Cache
{
  public sealed class VersionsCache
  {
    private DBCache _dbCache;
    private Func<IRepositoryAware, Task<Release>> _releaseRetriever;
    public VersionsCache(Func<IRepositoryAware, Task<Release>> releaseRetriever)
    {
      _releaseRetriever = releaseRetriever;
      _dbCache = new DBCache(releaseRetriever);
    }
    public async Task<string> Get(IRepositoryAware repositoryAware) =>
      await _dbCache.Get(repositoryAware);
  }
}