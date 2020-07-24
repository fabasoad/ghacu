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
        int result = ToSemVersion().CompareTo(other.ToSemVersion());
        return result == 0 ? string.Compare(Value, other.Value, StringComparison.Ordinal) : result;
      }
      catch (Exception)
      {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
      }
    }

    public SemVersion ToSemVersion() => SemVersion.Parse(Value?.StartsWith("v") ?? false ? Value.Substring(1) : Value);
  }
}