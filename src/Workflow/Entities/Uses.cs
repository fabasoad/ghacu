using System;
using Semver;

namespace GHACU.Workflow.Entities
{
  public sealed class Uses : IUses
  {
    public Uses(string fullName)
    {
      Type = fullName.StartsWith("docker://") ? UsesType.DOCKER : UsesType.GITHUB;
      var splitted = Type switch
      {
        UsesType.DOCKER => fullName.Substring(9).Split(':'),
        _ => fullName.Split('@')
      };
      FullName = splitted[0];
      CurrentVersion = ToSemVersion(splitted[1]);
    }

    public string FullName { get; }

    public string Owner => FullName.Split('/')[0];

    public string ActionName => FullName.Split('/')[1];

    public SemVersion CurrentVersion { get; }

    public SemVersion LatestVersion { get; private set; }

    public UsesType Type { get; }

    public void UpdateLatestVersion(string version)
    {
      LatestVersion = ToSemVersion(version);
    }

    private SemVersion ToSemVersion(string version) =>
      SemVersion.Parse(version.StartsWith('v') ? version.Substring(1) : version);
  }
}