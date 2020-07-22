using System;
using System.Collections.Generic;
using Xunit;
using Version = Ghacu.Api.Entities.Version;

namespace Ghacu.Api.Tests.Entities
{
  public class VersionTest
  {
    [Theory]
    [InlineData("v45.3.0-beta.3+8908")]
    [InlineData("45.3.0-beta.3+8908")]
    public void ToSemVersion_Valid(string v)
    {
      var version = new Version(v).ToSemVersion();
      Assert.Equal(45, version.Major);
      Assert.Equal(3, version.Minor);
      Assert.Equal(0, version.Patch);
      Assert.Equal("beta.3", version.Prerelease);
      Assert.Equal("8908", version.Build);
    }
    
    [Theory]
    [MemberData(nameof(DataToSemVersionInvalid))]
    public void ToSemVersion_Invalid(string v, Type exceptionType) =>
      Assert.Throws(exceptionType, () => new Version(v).ToSemVersion());

    public static IEnumerable<object[]> DataToSemVersionInvalid() => new List<object[]>
    {
      new object[] { null, typeof(ArgumentNullException) },
      new object[] { "vInvalid", typeof(ArgumentException) },
      new object[] { "Invalid", typeof(ArgumentException) }
    };

    [Theory]
    [MemberData(nameof(DataCompareToAllCases))]
    public void CompareTo_AllCases(Version v1, Version v2, int expected) =>
      Assert.Equal(expected, v1.CompareTo(v2));

    public static IEnumerable<object[]> DataCompareToAllCases()
    {
      var v1 = new Version("v1.1.1-preview.5+56");
      return new List<object[]>
      {
        // Same by reference
        new object[] { v1, v1, 0 },
        new object[] { v1, null, 1 },
        // Same by value
        new object[] { v1, new Version("v1.1.1-preview.5+56"), 0 },
        // Major
        new object[] { v1, new Version("v2.1.1"), -1 },
        new object[] { v1, new Version("v0.1.1"), 1 },
        // Minor
        new object[] { v1, new Version("v1.2.1"), -1 },
        new object[] { v1, new Version("v1.0.1"), 1 },
        // Patch
        new object[] { v1, new Version("v1.1.2"), -1 },
        new object[] { v1, new Version("v1.1.0"), 1 },
        // Prerelease
        new object[] { v1, new Version("v1.1.1-preview.6"), -1 },
        new object[] { v1, new Version("v1.1.1-preview.4"), 1 },
        // Build
        new object[] { v1, new Version("v1.1.1-preview.5+57"), -1 },
        new object[] { v1, new Version("v1.1.1-preview.5+55"), 1 },
        // Invalid
        new object[] { new Version("not-valid-1"), new Version("not-valid-1"), 0 },
        new object[] { new Version("not-valid-1"), new Version("not-valid-2"), -1 },
        new object[] { new Version("not-valid-2"), new Version("not-valid-1"), 1 },
      };
    }
  }
}