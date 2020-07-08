using Ghacu.Api;

namespace Ghacu.Runner
{
  public class GlobalConfig : IGlobalConfig
  {
    public string GitHubToken { get; }
    public bool UseCache { get; }

    public GlobalConfig(string token, bool useCache)
    {
      GitHubToken = token;
      UseCache = useCache;
    }
  }
}