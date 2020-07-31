using System;
using System.IO;
using System.Threading.Tasks;
using Ghacu.Api.Version;
using LiteDB;
using Microsoft.Extensions.Logging;

namespace Ghacu.Cache
{
  public sealed class DbCacheVersionProvider : IDbCacheVersionProvider
  {
    internal const string DB_NAME = "e6DF9AfAmX1Sy7zHCX07VP_1";
    private const string ACTIONS_COLLECTION = "actions";
    private readonly Func<string, ILiteDatabase> _databaseFactory;
    private readonly ILogger<DbCacheVersionProvider> _logger;
    private readonly IGitHubVersionProvider _provider;
    private readonly TimeSpan _storageTime = TimeSpan.FromMinutes(1);

    public DbCacheVersionProvider(
      ILoggerFactory loggerFactory,
      IGitHubVersionProvider versionProvider,
      Func<string, ILiteDatabase> databaseFactory)
    {
      _logger = loggerFactory.CreateLogger<DbCacheVersionProvider>();
      _provider = versionProvider;
      _databaseFactory = databaseFactory;
    }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      using ILiteDatabase db = _databaseFactory(GetDbFilePath());
      string actionName = $"{owner}/{repository}";
      ILiteCollection<ActionDto> actions = db.GetCollection<ActionDto>(ACTIONS_COLLECTION);
      ActionDto actionDto = actions.FindById(actionName);
      if (actionDto == null)
      {
        actionDto = new ActionDto
        {
          Name = actionName,
          Version = await _provider.GetLatestVersionAsync(owner, repository),
          Timestamp = DateTime.Now
        };
        actions.Insert(actionName, actionDto);
      }
      else if (DateTime.Now.Subtract(actionDto.Timestamp) > _storageTime)
      {
        actionDto.Version = await _provider.GetLatestVersionAsync(owner, repository);
        actionDto.Timestamp = DateTime.Now;
        actions.Update(actionName, actionDto);
      }
      else
      {
        _logger.LogInformation($"{owner}/{repository} version is retrieved from local DB");
      }

      return actionDto.Version;
    }

    private static string GetDbFilePath()
    {
      string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string ghacuFolder = Path.Combine(appData, "ghacu");
      if (!Directory.Exists(ghacuFolder))
      {
        Directory.CreateDirectory(ghacuFolder);
      }

      return Path.Combine(ghacuFolder, DB_NAME);
    }
  }
}