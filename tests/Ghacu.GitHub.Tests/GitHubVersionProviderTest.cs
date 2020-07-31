using System;
using System.Net;
using Ghacu.Api.Stream;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Octokit;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.GitHub.Tests
{
  public class GitHubVersionProviderTest
  {
    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseExists()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .TaskResult(expected);
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(Arg.AnyString, Arg.AnyString))
        .OccursNever();
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      string actual = await provider.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      Mock.Assert(gitHubClientMock);
    }
    
    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseIsNull()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .TaskResult(null);
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(Arg.AnyString, Arg.AnyString))
        .OccursNever();
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      await Assert.ThrowsAsync<GitHubVersionNotFoundException>(
        () => provider.GetLatestVersionAsync(owner, repository));
      Mock.Assert(gitHubClientMock);
    }
    
    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseIsNotFound_LatestTagExists()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .Throws(new NotFoundException(null, HttpStatusCode.NotFound))
        .OccursOnce();
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(owner, repository))
        .TaskResult(expected);
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      string actual = await provider.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      Mock.Assert(gitHubClientMock);
    }
    
    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseIsNotFound_LatestTagIsNull()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .Throws(new NotFoundException(null, HttpStatusCode.NotFound))
        .OccursOnce();
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(owner, repository))
        .TaskResult(null)
        .OccursOnce();
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      await Assert.ThrowsAsync<GitHubVersionNotFoundException>(
        () => provider.GetLatestVersionAsync(owner, repository));
      Mock.Assert(gitHubClientMock);
    }

    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseInternalError()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .Throws<Exception>();
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(Arg.AnyString, Arg.AnyString))
        .OccursNever();
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      await Assert.ThrowsAsync<GitHubVersionNotFoundException>(
        () => provider.GetLatestVersionAsync(owner, repository));
      Mock.Assert(gitHubClientMock);
    }

    [Fact]
    public async void GetLatestVersionAsync_LatestReleaseIsNotFound_LatestTagInternalError()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      
      var gitHubClientMock = Mock.Create<IGitHubClient>();
      Mock.Arrange(() => gitHubClientMock.GetLatestReleaseVersionAsync(owner, repository))
        .Throws(new NotFoundException(null, HttpStatusCode.NotFound))
        .OccursOnce();
      Mock.Arrange(() => gitHubClientMock.GetLatestTagVersionAsync(Arg.AnyString, Arg.AnyString))
        .Throws<Exception>()
        .OccursOnce();
      
      var provider = new GitHubVersionProvider(gitHubClientMock, Mock.Create<IStreamer>());
      await Assert.ThrowsAsync<GitHubVersionNotFoundException>(
        () => provider.GetLatestVersionAsync(owner, repository));
      Mock.Assert(gitHubClientMock);
    }
  }
}