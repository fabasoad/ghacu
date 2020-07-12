using System;
using Semver;
using YamlDotNet.Serialization;

namespace Ghacu.Api.Entities
{
  public sealed class Step
  {
    private Uses _uses;

    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }

    public Uses Uses => _uses ??= new Uses(UsesFullName);
    public bool IsInternal => UsesFullName == null || "./".Equals(UsesFullName);
    public bool IsUpToDate => IsInternal || Uses.CurrentVersion.CompareTo(Uses.GetLatestVersion()) >= 0;

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
          currentVersion = Uses.CurrentVersion.ToSemVersion();
          latestVersion = Uses.GetLatestVersion().ToSemVersion();
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

        if (!string.Equals(currentVersion.Prerelease, latestVersion.Prerelease, StringComparison.Ordinal))
        {
          return VersionDiffType.Prerelease;
        }

        return string.Equals(currentVersion.Build, latestVersion.Build, StringComparison.Ordinal)
          ? VersionDiffType.None
          : VersionDiffType.Build;
      }
    }
  }
}