using Ghacu.Api;

namespace Ghacu.Runner
{
  public class GlobalConfig : IGlobalConfig
  {
    public GlobalConfig(string token, bool useCache)
    {
      GitHubToken = token;
      UseCache = useCache;
    }

    public string GitHubToken { get; }
    public bool UseCache { get; }
  }
}