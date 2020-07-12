using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Ghacu.Api;
using LiteDB;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.Cache.Tests
{
  public class DbCacheTest
  {
    [Fact]
    public async Task GetLatestVersionAsync_NoDto()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string version = "test-version";
      string actionName = $"{owner}/{repository}";

      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.FromResult(version));

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.GitHub, type);
        return latestVersionProviderMock;
      }

      var dtoListMock = Mock.Create<ILiteCollection<ActionDto>>();
      Mock.Arrange(() => dtoListMock.FindById(actionName)).Returns<ActionDto>(null);
      Mock.Arrange(() => dtoListMock.Insert(actionName, Arg.Matches<ActionDto>(
        dto => dto.Name == actionName && dto.Version == version && DateTime.Now.CompareTo(dto.Timestamp) >= 0)));
      Mock.Arrange(() => dtoListMock.Update(Arg.AnyString, Arg.IsAny<ActionDto>())).OccursNever();

      var liteDatabaseMock = Mock.Create<ILiteDatabase>();
      Mock.Arrange(() => liteDatabaseMock.GetCollection<ActionDto>("actions", Arg.IsAny<BsonAutoId>()))
        .Returns(dtoListMock);

      ILiteDatabase DatabaseFactory(string dbPath)
      {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string ghacuFolder = Path.Combine(appData, "ghacu");
        Assert.Equal(Path.Combine(ghacuFolder, DbCache.DB_NAME), dbPath);
        return liteDatabaseMock;
      }

      var dbCache = new DbCache(new LoggerFactory(), LatestVersionProviderFactory, DatabaseFactory);
      string actualVersion = await dbCache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(version, actualVersion);
      Mock.Assert(dtoListMock);
    }

    [Fact]
    public async Task GetLatestVersionAsync_InvalidCachedDto()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string version = "test-version";
      string actionName = $"{owner}/{repository}";

      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.FromResult(version));

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.GitHub, type);
        return latestVersionProviderMock;
      }

      var actionDto = new ActionDto { Name = actionName, Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(1)) };

      var dtoListMock = Mock.Create<ILiteCollection<ActionDto>>();
      Mock.Arrange(() => dtoListMock.FindById(actionName)).Returns(actionDto);
      Mock.Arrange(() => dtoListMock.Insert(Arg.AnyString, Arg.IsAny<ActionDto>())).OccursNever();
      Mock.Arrange(() => dtoListMock.Update(actionName, Arg.Matches<ActionDto>(
        dto => dto.Name == actionName && dto.Version == version && DateTime.Now.CompareTo(dto.Timestamp) >= 0)));

      var liteDatabaseMock = Mock.Create<ILiteDatabase>();
      Mock.Arrange(() => liteDatabaseMock.GetCollection<ActionDto>("actions", Arg.IsAny<BsonAutoId>()))
        .Returns(dtoListMock);

      ILiteDatabase DatabaseFactory(string dbPath)
      {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string ghacuFolder = Path.Combine(appData, "ghacu");
        Assert.Equal(Path.Combine(ghacuFolder, DbCache.DB_NAME), dbPath);
        return liteDatabaseMock;
      }

      var dbCache = new DbCache(new LoggerFactory(), LatestVersionProviderFactory, DatabaseFactory);
      string actualVersion = await dbCache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(version, actualVersion);
      Mock.Assert(dtoListMock);
    }

    [Fact]
    public async Task GetLatestVersionAsync_ValidCachedDto()
    {
      const string owner = "test-owner";
      const string repository = "test-repository";
      const string version = "test-version";
      string actionName = $"{owner}/{repository}";

      var latestVersionProviderMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => latestVersionProviderMock.GetLatestVersionAsync(owner, repository))
        .Returns(Task.FromResult(version));

      ILatestVersionProvider LatestVersionProviderFactory(LatestVersionProviderType type)
      {
        Assert.Equal(LatestVersionProviderType.GitHub, type);
        return latestVersionProviderMock;
      }

      var actionDto = new ActionDto
      {
        Name = actionName, Timestamp = DateTime.Now, Version = version
      };

      var dtoListMock = Mock.Create<ILiteCollection<ActionDto>>();
      Mock.Arrange(() => dtoListMock.FindById(actionName)).Returns(actionDto);
      Mock.Arrange(() => dtoListMock.Insert(Arg.AnyString, Arg.IsAny<ActionDto>())).OccursNever();
      Mock.Arrange(() => dtoListMock.Update(Arg.AnyString, Arg.IsAny<ActionDto>())).OccursNever();

      var liteDatabaseMock = Mock.Create<ILiteDatabase>();
      Mock.Arrange(() => liteDatabaseMock.GetCollection<ActionDto>("actions", Arg.IsAny<BsonAutoId>()))
        .Returns(dtoListMock);

      ILiteDatabase DatabaseFactory(string dbPath)
      {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string ghacuFolder = Path.Combine(appData, "ghacu");
        Assert.Equal(Path.Combine(ghacuFolder, DbCache.DB_NAME), dbPath);
        return liteDatabaseMock;
      }

      var dbCache = new DbCache(new LoggerFactory(), LatestVersionProviderFactory, DatabaseFactory);
      string actualVersion = await dbCache.GetLatestVersionAsync(owner, repository);
      Assert.Equal(version, actualVersion);
      Mock.Assert(dtoListMock);
    }

    [Theory]
    [MemberData(nameof(DataDatabaseFolderExistence))]
    public void GetLatestVersionAsync_DatabaseFolderExistence(Action setUp)
    {
      setUp();

      static ILiteDatabase DatabaseFactory(string dbPath)
      {
        Assert.True(Directory.Exists(dbPath));
        throw new DbCacheMockException();
      }

      var dbCache = new DbCache(new LoggerFactory(), _ => null, DatabaseFactory);
      Assert.ThrowsAsync<DbCacheMockException>(async () =>
        await dbCache.GetLatestVersionAsync(string.Empty, string.Empty));
    }

    [SuppressMessage("ReSharper", "SA1201")]
    public static IEnumerable<object[]> DataDatabaseFolderExistence
    {
      get
      {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string ghacuFolder = Path.Combine(appData, "ghacu");
        return new List<object[]>
        {
          new object[]
          {
            new Action(() =>
            {
              if (!Directory.Exists(ghacuFolder))
              {
                Directory.CreateDirectory(ghacuFolder);
              }
            })
          },
          new object[]
          {
            new Action(() =>
            {
              if (Directory.Exists(ghacuFolder))
              {
                Directory.Delete(ghacuFolder, true);
              }
            })
          }
        };
      }
    }

    public class DbCacheMockException : Exception
    {
    }
  }
}