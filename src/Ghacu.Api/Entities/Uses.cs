using System;

namespace Ghacu.Api.Entities
{
  public sealed class Uses : IEquatable<Uses>
  {
    private Version _latestVersion;

    public Uses(string fullName)
    {
      Type = fullName.StartsWith("docker://") ? UsesType.Docker : UsesType.GitHub;
      string[] fullNameParts = Type switch
      {
        UsesType.Docker => fullName.Substring(9).Split(':'),
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

    public bool Equals(Uses other)
    {
      if (ReferenceEquals(null, other))
      {
        return false;
      }

      if (ReferenceEquals(this, other))
      {
        return true;
      }

      return FullName == other.FullName && Type == other.Type;
    }

    public override bool Equals(object obj) =>
      ReferenceEquals(this, obj) || obj is Uses other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(FullName, (int) Type);
  }
}