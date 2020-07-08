namespace Ghacu.Api
{
  public interface IGlobalConfig
  {
    public string GitHubToken { get; }
    public bool UseCache { get; }
  }
}