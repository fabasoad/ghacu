namespace Ghacu.Api.Entities
{
  public sealed class Uses
  {
    private Version _latestVersion;

    public Uses(string fullName)
    {
      Type = fullName.StartsWith("docker://") ? UsesType.DOCKER : UsesType.GITHUB;
      string[] fullNameParts = Type switch
      {
        UsesType.DOCKER => fullName.Substring(9).Split(':'),
        _ => fullName.Split('@')
      };
      FullName = fullNameParts[0];
      CurrentVersion = new Version(fullNameParts[1]);
    }

    public string FullName { get; }

    public string Owner => FullName.Split('/')[0];

    public string ActionName => FullName.Split('/')[1];

    public Version CurrentVersion { get; }

    public UsesType Type { get; }

    public Version GetLatestVersion()
    {
      return _latestVersion;
    }

    public void SetLatestVersion(string version)
    {
      _latestVersion = new Version(version);
    }
  }
}