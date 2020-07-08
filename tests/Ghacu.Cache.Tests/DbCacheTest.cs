namespace Ghacu.Runner.Tests.Cache
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Ghacu.Api;
  using Ghacu.Runner.Cache;
  using LiteDB;
  using Microsoft.Extensions.Logging;
  using Telerik.JustMock;
  using Xunit;

  public class DbCacheTest
  {
    [Fact]
    public async Task GetLatestVersion_NoDto()
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
        Assert.Equal(LatestVersionProviderType.GITHUB, type);
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
    }

    [Fact]
    public async Task GetLatestVersion_OldCachedDto()
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
        Assert.Equal(LatestVersionProviderType.GITHUB, type);
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
    }
  }
}