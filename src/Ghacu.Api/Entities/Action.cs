using System;
using Semver;

namespace Ghacu.Api.Entities
{
  public sealed class Action : IEquatable<Action>
  {
    private Version _currentVersion;
    private Version _latestVersion;
    
    public Action(string fullName)
    {
      Type = string.IsNullOrEmpty(fullName) || fullName == "./"
        ? UsesType.Internal
        : fullName.StartsWith("docker://") ? UsesType.Docker : UsesType.GitHub;
      string[] fullNameParts = Type switch
      {
        UsesType.Docker => fullName.Substring(9).Split(':'),
        UsesType.GitHub => fullName.Split('@'),
        _ => new[] { fullName, string.Empty }
      };
      FullName = fullNameParts[0];
      _currentVersion = new Version(fullNameParts[1]);
      if (Type == UsesType.Internal)
      {
        _latestVersion = new Version(fullNameParts[1]);
      }
    }

    public string FullName { get; }

    public string Owner => FullName.Split('/')[0];

    public string ActionName => FullName.Split('/')[1];

    public string CurrentVersion => _currentVersion.Value;

    public UsesType Type { get; }
    
    public bool IsUpToDate => _currentVersion.CompareTo(_latestVersion) >= 0;

    public string LatestVersion
    {
      get => _latestVersion?.Value;
      set => _latestVersion = new Version(value);
    }
    
    public VersionDiffType VersionDiffType
    {
      get
      {
        if (IsUpToDate)
        {
          return VersionDiffType.None;
        }

        SemVersion currentVersion;
        SemVersion latestVersion;
        try
        {
          currentVersion = _currentVersion.ToSemVersion();
          latestVersion = _latestVersion.ToSemVersion();
        }
        catch
        {
          return VersionDiffType.None;
        }

        if (currentVersion.Major != latestVersion.Major)
        {
          return VersionDiffType.Major;
        }

        if (currentVersion.Minor != latestVersion.Minor)
        {
          return VersionDiffType.Minor;
        }

        if (currentVersion.Patch != latestVersion.Patch)
        {
          return VersionDiffType.Patch;
        }

        if (currentVersion.Prerelease != latestVersion.Prerelease)
        {
          return VersionDiffType.Prerelease;
        }

        return currentVersion.Build == latestVersion.Build ? VersionDiffType.None : VersionDiffType.Build;
      }
    }

    public bool Equals(Action other)
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
      ReferenceEquals(this, obj) || obj is Action other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(FullName, (int)Type);
  }
}