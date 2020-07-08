using System;
using Semver;

namespace Ghacu.Api.Entities
{
  public class Version : IComparable<Version>
  {
    public Version(string value)
    {
      Value = value;
    }

    public string Value { get; }

    public int CompareTo(Version other)
    {
      if (ReferenceEquals(this, other))
      {
        return 0;
      }

      if (ReferenceEquals(null, other))
      {
        return 1;
      }

      try
      {
        return ToSemVersion(Value).CompareByPrecedence(ToSemVersion(other.Value));
      }
      catch (Exception)
      {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
      }
    }

    private static SemVersion ToSemVersion(string version)
    {
      return SemVersion.Parse(version.StartsWith('v') ? version.Substring(1) : version);
    }
  }
}