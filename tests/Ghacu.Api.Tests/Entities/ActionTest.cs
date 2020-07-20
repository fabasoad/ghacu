using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class ActionTest
  {
    private const string EXPECTED_OWNER = "own1";
    private const string EXPECTED_REPOSITORY = "test.repo1";

    [Theory]
    [MemberData(nameof(DataInitializeAllCases))]
    public void Initialize_AllCasesWithoutLatestVersion(
      string actionName,
      UsesType type,
      string fullName,
      string owner,
      string repository,
      string currentVersion,
      string latestVersion,
      bool isValidForUpgrade,
      bool isUpToDate,
      VersionDiffType versionDiffType)
    {
      var action = new Action(actionName);
      Assert.Equal(type, action.Type);
      Assert.Equal(fullName, action.FullName);
      Assert.Equal(owner, action.Owner);
      Assert.Equal(repository, action.Repository);
      Assert.Equal(currentVersion, action.CurrentVersion);
      Assert.Equal(latestVersion, action.LatestVersion);
      Assert.Equal(isValidForUpgrade, action.IsValidForUpgrade);
      Assert.Equal(isUpToDate, action.IsUpToDate);
      Assert.Equal(versionDiffType, action.VersionDiffType);
    }

    [Theory]
    [MemberData(nameof(DataInitializeAllCasesWithLatestVersion))]
    public void Initialize_AllCasesWithLatestVersion(
      string actionName,
      UsesType type,
      string fullName,
      string owner,
      string repository,
      string currentVersion,
      string latestVersion,
      bool isValidForUpgrade,
      bool isUpToDate,
      VersionDiffType versionDiffType)
    {
      var action = new Action(actionName) {LatestVersion = latestVersion};
      Assert.Equal(type, action.Type);
      Assert.Equal(fullName, action.FullName);
      Assert.Equal(owner, action.Owner);
      Assert.Equal(repository, action.Repository);
      Assert.Equal(currentVersion, action.CurrentVersion);
      Assert.Equal(latestVersion, action.LatestVersion);
      Assert.Equal(isValidForUpgrade, action.IsValidForUpgrade);
      Assert.Equal(isUpToDate, action.IsUpToDate);
      Assert.Equal(versionDiffType, action.VersionDiffType);
    }

    public static IEnumerable<object[]> DataInitializeAllCases => new List<object[]>
    {
      new object[]
      {
        $"docker://{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}:latest",
        UsesType.Docker,
        $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
        EXPECTED_OWNER,
        EXPECTED_REPOSITORY,
        "latest",
        null,
        true,
        true,
        VersionDiffType.None
      },
      new object[]
      {
        $"docker://{EXPECTED_REPOSITORY}:latest",
        UsesType.Docker,
        EXPECTED_REPOSITORY,
        null,
        EXPECTED_REPOSITORY,
        "latest",
        null,
        true,
        true,
        VersionDiffType.None
      },
      new object[]
      {
        $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}@v1.2.3",
        UsesType.GitHub,
        $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
        EXPECTED_OWNER,
        EXPECTED_REPOSITORY,
        "v1.2.3",
        null,
        true,
        true,
        VersionDiffType.None
      },
      new object[]
      {
        "./",
        UsesType.Internal,
        "./",
        null,
        "./",
        null,
        null,
        false,
        true,
        VersionDiffType.None
      },
      new object[]
      {
        null,
        UsesType.Unknown,
        null,
        null,
        null,
        null,
        null,
        false,
        true,
        VersionDiffType.None
      },
      new object[]
      {
        "invalid-string",
        UsesType.Unknown,
        "invalid-string",
        null,
        "invalid-string",
        null,
        null,
        false,
        true,
        VersionDiffType.None
      }
    };

    public static IEnumerable<object[]> DataInitializeAllCasesWithLatestVersion =>
      DataInitializeAllCases.Concat(new List<object[]>
      {
        new object[]
        {
          $"docker://{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}:v1.0.0",
          UsesType.Docker,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v1.0.0",
          "v1.0.1",
          true,
          false,
          VersionDiffType.Patch
        },
        new object[]
        {
          $"docker://{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}:v2.1.67",
          UsesType.Docker,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v2.1.67",
          "v2.2.16",
          true,
          false,
          VersionDiffType.Minor
        },
        new object[]
        {
          $"docker://{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}:v3",
          UsesType.Docker,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v3",
          "v4.5",
          true,
          false,
          VersionDiffType.Major
        },
        new object[]
        {
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}@v42.224.1-alpha",
          UsesType.GitHub,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v42.224.1-alpha",
          "v42.224.1-beta",
          true,
          false,
          VersionDiffType.Prerelease
        },
        new object[]
        {
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}@v1.3.1-b1ef23r+23489756",
          UsesType.GitHub,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v1.3.1-b1ef23r+23489756",
          "v1.3.1-b1ef23r+23489757",
          true,
          false,
          VersionDiffType.Build
        },
        new object[]
        {
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}@v12.3.1-ek2ng44+456",
          UsesType.GitHub,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v12.3.1-ek2ng44+456",
          "v12.3.1-ek2ng44+456",
          true,
          true,
          VersionDiffType.None
        },
        new object[]
        {
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}@v12.3.1-ek2ng44+456",
          UsesType.GitHub,
          $"{EXPECTED_OWNER}/{EXPECTED_REPOSITORY}",
          EXPECTED_OWNER,
          EXPECTED_REPOSITORY,
          "v12.3.1-ek2ng44+456",
          "test-test-test-test",
          true,
          true,
          VersionDiffType.None
        }
      });

    [Theory]
    [MemberData(nameof(DataEqualsAllCases))]
    public void Equals_AllCases(Action action1, Action action2, bool expected)
    {
      Assert.Equal(expected, action1.Equals(action2));
      Assert.Equal(expected, action1.Equals((object)action2));
    }

    public static IEnumerable<object[]> DataEqualsAllCases
    {
      get
      {
        var actionUnknown1 = new Action("unknown-1");
        return new List<object[]>
        {
          new object[] { new Action("docker://repo1:v45.4.4"), new Action("docker://repo1:v45.4.4"), true },
          new object[] { new Action("docker://owner/repo1:latest"), new Action("docker://owner/repo1:v45.4.4"), true },
          new object[] { new Action("owner/repo1@latest"), new Action("docker://owner/repo1:v45.4.4"), false },
          new object[] { new Action("owner/repo1@latest"), new Action("./"), false },
          new object[] { actionUnknown1, new Action("unknown-2"), false },
          new object[] { actionUnknown1, new Action("unknown-1"), true },
          new object[] { actionUnknown1, actionUnknown1, true },
          new object[] { actionUnknown1, null, false },
        };
      }
    }

    [Fact]
    public void GetHashCode_AllCases()
    {
      var a1 = new Action("docker://repo1:v45.4.4");
      var a2 = new Action("docker://repo1:v45.4.4");
      var a3 = new Action("docker://repo1:n1fi3fr");
      var a4 = new Action("owner1/repo1@n1fi3fr");
      var a5 = new Action("./");
      var a6 = new Action("unknown");
      var a7 = new Action(null);
      Assert.Equal(a1.GetHashCode(), a2.GetHashCode());
      Assert.Equal(a2.GetHashCode(), a3.GetHashCode());
      Assert.NotEqual(a3.GetHashCode(), a4.GetHashCode());
      Assert.NotEqual(a4.GetHashCode(), a5.GetHashCode());
      Assert.NotEqual(a3.GetHashCode(), a5.GetHashCode());
      Assert.NotEqual(a3.GetHashCode(), a6.GetHashCode());
      Assert.NotEqual(a4.GetHashCode(), a6.GetHashCode());
      Assert.NotEqual(a5.GetHashCode(), a6.GetHashCode());
      Assert.NotEqual(a3.GetHashCode(), a7.GetHashCode());
      Assert.NotEqual(a4.GetHashCode(), a7.GetHashCode());
      Assert.NotEqual(a5.GetHashCode(), a7.GetHashCode());
      Assert.NotEqual(a6.GetHashCode(), a7.GetHashCode());
    }
  }
}