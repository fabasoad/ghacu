using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHApi
{
  public class RepositoryScanner
  {
    private IDictionary<string, RepositoryInfo> _cache = new Dictionary<string, RepositoryInfo>();
    public RepositoryInfo Scan(string name)
    {
      if (!_cache.ContainsKey(name))
      {
        var result = new RepositoryInfo(name);
        result.Tags = new List<string>();
        _cache.Add(name, result);
      }
      return _cache[name];
    }
  }
}