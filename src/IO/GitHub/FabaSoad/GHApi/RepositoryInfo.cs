using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHApi
{
  public class RepositoryInfo
  {
    internal RepositoryInfo(string name)
    {
      Name = name;
    }
    public string Name { get; private set; }
    public IEnumerable<string> Tags { get; internal set; }
  }
}