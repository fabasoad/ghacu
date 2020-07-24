using Ghacu.Api;

namespace Ghacu.Runner
{
  public class GlobalConfig : IGlobalConfig
  {
    public GlobalConfig(bool useCache)
    {
      UseCache = useCache;
    }

    public bool UseCache { get; }
  }
}