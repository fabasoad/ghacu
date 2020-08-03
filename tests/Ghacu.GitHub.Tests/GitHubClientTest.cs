using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Octokit;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.GitHub.Tests
{
  public class GitHubClientTest
  {
    [Fact]
    public async void GetLatestReleaseVersionAsync_Positive()
    {
      const string owner = "test-owner";
      const string name = "test-name";
      const string expected = "test-version";

      var octokitRelease = new Release(
        default,
        default,
        default,
        default,
        default,
        default,
        expected,
        default,
        default,
        default,
        default,
        default,
        default,
        default(DateTimeOffset),
        default,
        default,
        default,
        new List<ReleaseAsset>());

      var octokitReleaseClientMock = Mock.Create<IReleasesClient>();
      Mock.Arrange(() => octokitReleaseClientMock.GetLatest(owner, name))
        .Returns(Task.FromResult(octokitRelease));
      
      var octokitRepositoryMock = Mock.Create<IRepositoriesClient>();
      Mock.Arrange(() => octokitRepositoryMock.Release).Returns(octokitReleaseClientMock);
      
      var octokitGitHubClientMock = Mock.Create<Octokit.IGitHubClient>();
      Mock.Arrange(() => octokitGitHubClientMock.Repository).Returns(octokitRepositoryMock);
      
      var gitHubClient = new GitHubClient(octokitGitHubClientMock);
      string actual = await gitHubClient.GetLatestReleaseVersionAsync(owner, name);
      Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(DataGetLatestTagVersionAsyncPositive))]
    public async void GetLatestTagVersionAsync_Positive(IReadOnlyList<RepositoryTag> tags, string expected)
    {
      const string owner = "test-owner";
      const string name = "test-name";
      
      var octokitRepositoryMock = Mock.Create<IRepositoriesClient>();
      Mock.Arrange(() => octokitRepositoryMock.GetAllTags(owner, name))
        .Returns(Task.FromResult(tags));
      
      var octokitGitHubClientMock = Mock.Create<Octokit.IGitHubClient>();
      Mock.Arrange(() => octokitGitHubClientMock.Repository).Returns(octokitRepositoryMock);
      
      var gitHubClient = new GitHubClient(octokitGitHubClientMock);
      string actual = await gitHubClient.GetLatestTagVersionAsync(owner, name);
      Assert.Equal(expected, actual);
    }

    [SuppressMessage("ReSharper", "SA1201")]
    public static IEnumerable<object[]> DataGetLatestTagVersionAsyncPositive => new List<object[]>
    {
      new object[]
      {
        new List<RepositoryTag>
        {
          new RepositoryTag(
            "test-version-1", default, default, default, default),
          new RepositoryTag(
            "test-version-2", default, default, default, default),
        },
        "test-version-2"
      },
      new object[]
      {
        new List<RepositoryTag>
        {
          new RepositoryTag(
            "test-version", default, default, default, default)
        },
        "test-version"
      },
      new object[] { new List<RepositoryTag>(), null },
      new object[] { null, null }
    };

    [Fact]
    public void Create_Successfully()
    {
      const string appName = "test-app";
      const string token = "test-token";
      
      var gitHubClient = new GitHubClient(appName, token);
      Assert.IsType<Connection>(gitHubClient.OctokitClient.Connection);
      var octokitConnection = gitHubClient.OctokitClient.Connection as Connection;
      Assert.True(octokitConnection?.UserAgent.Contains(appName));
      Assert.IsType<Octokit.GitHubClient>(gitHubClient.OctokitClient);
      var octokitClient = gitHubClient.OctokitClient as Octokit.GitHubClient;
      Assert.Equal(token, octokitClient.Credentials.GetToken());
    }
  }
}