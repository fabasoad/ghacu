using System;
using System.Threading;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.Runner.Cache;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.Runner.Tests.Cache
{
  public class MemoryCacheTest
  {
    [Fact]
    public async void GetLatestVersionAsync_ValidCachedVersion()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock
          .GetLatestVersionAsync(Arg.AnyString, Arg.AnyString))
        .OccursNever();

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.DbCache, type);
        return latestVersionProviderMock;
      }

      var semaphoreMock = Mock.Create<ISemaphore>();
      Mock.Arrange(() => semaphoreMock.WaitAsync()).OccursNever();
      Mock.Arrange(() => semaphoreMock.Release()).OccursNever();
      
      var cache = new MemoryCache(new LoggerFactory(), LatestVersionProviderFactory, semaphoreMock);
      cache.LocalCache.Add($"{owner}/{repository}", Task.FromResult(expected));
      string actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      
      Mock.Assert(latestVersionProviderMock);
      Mock.Assert(semaphoreMock);
    }

    [Fact]
    public async void GetLatestVersionAsync_ShouldNotGetFromProviderMoreThanOnce()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.FromResult(expected))
        .OccursOnce();

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.DbCache, type);
        return latestVersionProviderMock;
      }

      var semaphoreMock = Mock.Create<ISemaphore>();
      Mock.Arrange(() => semaphoreMock.WaitAsync()).Returns(Task.CompletedTask).OccursOnce();
      Mock.Arrange(() => semaphoreMock.Release()).Returns(1 /* any int */).OccursOnce();

      var cache = new MemoryCache(new LoggerFactory(), LatestVersionProviderFactory, semaphoreMock);
      string actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      // Check that second call will get value from cache
      actual = await cache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(expected, actual);
      
      Mock.Assert(latestVersionProviderMock);
      Mock.Assert(semaphoreMock);
    }

    [Fact(Timeout = 5000)]
    public async void GetLatestVersionAsync_ShouldNotGetFromProviderAsynchronously()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string expected = "test-version";
      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.Run(async () =>
        {
          await Task.Delay(TimeSpan.FromMilliseconds(300));
          return expected;
        }))
        .OccursOnce();

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.DbCache, type);
        return latestVersionProviderMock;
      }
      
      var cache = new MemoryCache(new LoggerFactory(), LatestVersionProviderFactory, new SemaphoreSlimProxy());
      async void Run() => Assert.Equal(expected, await cache.GetLatestVersionAsync(owner, repository));
      
      await Task.Run(Run);
      await Task.Run(Run);
      
      Mock.Assert(latestVersionProviderMock);
    }
  }
}