using System;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.Api.Version;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.Cache.Tests
{
  public class MemoryCacheVersionProviderTest
  {
    [Fact]
    public async void GetLatestVersionAsync_ValidCachedVersion()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var versionProviderMock = Mock.Create<IDbCacheVersionProvider>();
      Mock.Arrange(() => versionProviderMock
          .GetLatestVersionAsync(Arg.AnyString, Arg.AnyString))
        .OccursNever();

      var semaphoreMock = Mock.Create<ISemaphoreSlimProxy>();
      Mock.Arrange(() => semaphoreMock.WaitAsync()).OccursNever();
      Mock.Arrange(() => semaphoreMock.Release()).OccursNever();
      
      var cache = new MemoryCacheVersionProvider(new LoggerFactory(), versionProviderMock, semaphoreMock);
      cache.LocalCache.Add($"{owner}/{repository}", Task.FromResult(expected));
      string actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      
      Mock.Assert(versionProviderMock);
      Mock.Assert(semaphoreMock);
    }

    [Fact]
    public async void GetLatestVersionAsync_ShouldNotGetFromProviderMoreThanOnce()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var versionProviderMock = Mock.Create<IDbCacheVersionProvider>();
      Mock.Arrange(() => versionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.FromResult(expected))
        .OccursOnce();

      var semaphoreMock = Mock.Create<ISemaphoreSlimProxy>();
      Mock.Arrange(() => semaphoreMock.WaitAsync()).Returns(Task.CompletedTask).OccursOnce();
      Mock.Arrange(() => semaphoreMock.Release()).Returns(1 /* any int */).OccursOnce();

      var cache = new MemoryCacheVersionProvider(new LoggerFactory(), versionProviderMock, semaphoreMock);
      string actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      // Check that second call will get value from cache
      actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      
      Mock.Assert(versionProviderMock);
      Mock.Assert(semaphoreMock);
    }

    [Fact(Timeout = 5000)]
    public async void GetLatestVersionAsync_ShouldNotGetFromProviderAsynchronously()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var versionProviderMock = Mock.Create<IDbCacheVersionProvider>();
      Mock.Arrange(() => versionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.Run(async () =>
        {
          await Task.Delay(TimeSpan.FromMilliseconds(300));
          return expected;
        }))
        .OccursOnce();
      
      var cache = new MemoryCacheVersionProvider(new LoggerFactory(), versionProviderMock, new SemaphoreSlimProxy());
      async void Run() => Assert.Equal(expected, await cache.GetLatestVersionAsync(owner, repository));
      
      await Task.Run(Run);
      await Task.Run(Run);
      
      Mock.Assert(versionProviderMock);
    }
  }
}