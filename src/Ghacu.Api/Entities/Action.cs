using System;
using System.Text.RegularExpressions;
using Semver;

namespace Ghacu.Api.Entities
{
  public sealed class Action : IEquatable<Action>
  {
    private const string DOCKER_PATTERN1 = "docker://(.+?)/(.+):(.+)";
    private const string DOCKER_PATTERN2 = "docker://(.+):(.+)";
    private const string GITHUB_PATTERN = "(.+?)/(.+)@(.+)";
    private const string INTERNAL_PATTERN = "(./)";

    private Version _currentVersion;
    private Version _latestVersion;

    public Action(string actionName)
    {
      if (actionName == null)
      {
        Type = UsesType.Unknown;
      }
      else
      {
        Match match = Regex.Match(actionName, DOCKER_PATTERN1);
        if (match.Success)
        {
          Type = UsesType.Docker;
          Initialize(
            match.Groups[1].Captures[0].Value,
            match.Groups[2].Captures[0].Value,
            match.Groups[3].Captures[0].Value);
          return;
        }

        match = Regex.Match(actionName, DOCKER_PATTERN2);
        if (match.Success)
        {
          Type = UsesType.Docker;
          Initialize(
            null,
            match.Groups[1].Captures[0].Value,
            match.Groups[2].Captures[0].Value);
          return;
        }

        match = Regex.Match(actionName, GITHUB_PATTERN);
        if (match.Success)
        {
          Type = UsesType.GitHub;
          Initialize(
            match.Groups[1].Captures[0].Value,
            match.Groups[2].Captures[0].Value,
            match.Groups[3].Captures[0].Value);
          return;
        }

        match = Regex.Match(actionName, INTERNAL_PATTERN);
        Type = match.Success ? UsesType.Internal : UsesType.Unknown;
      }

      Initialize(null, actionName, null);
    }

    private void Initialize(string owner, string repository, string version)
    {
      FullName = owner == null ? repository : $"{owner}/{repository}";
      Owner = owner;
      Repository = repository;
      _currentVersion = new Version(version);
      if (!IsValidForUpgrade)
      {
        _latestVersion = new Version(version);
      }
    }

    public string FullName { get; private set; }

    public string Owner { get; private set; }

    public string Repository { get; private set; }

    public string CurrentVersion => _currentVersion.Value;

    public UsesType Type { get; }

    public bool IsValidForUpgrade => Type == UsesType.Docker || Type == UsesType.GitHub;

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

    public override int GetHashCode() => HashCode.Combine(FullName, (int) Type);
  }
}