using System;
using System.Text.RegularExpressions;
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
        return ToSemVersion().CompareByPrecedence(other.ToSemVersion());
      }
      catch (Exception)
      {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
      }
    }

    public SemVersion ToSemVersion()
    {
      return SemVersion.Parse(Regex.IsMatch(Value, "v\\d\\.\\d.\\d.*") ? Value.Substring(1) : Value);
    }
  }
}