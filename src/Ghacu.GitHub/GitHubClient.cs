using System;
using System.Linq;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Octokit;
using OctokitClient = Octokit.GitHubClient;

namespace Ghacu.GitHub
{
  public sealed class GitHubClient : ILatestVersionProvider
  {
    private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";
    private const string APP_NAME = "ghacu";
    private readonly IGlobalConfig _globalConfig;
    private readonly ILogger<GitHubClient> _logger;
    private OctokitClient _client;

    public GitHubClient(ILoggerFactory loggerFactory, IGlobalConfig globalConfig)
    {
      _logger = loggerFactory.CreateLogger<GitHubClient>();
      _globalConfig = globalConfig;
    }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      _client ??= CreateGitHubClient();
      Exception lastException = null;
      string tagName = null;
      try
      {
        _logger.LogInformation($"Getting latest release for {owner}/{repository}...");
        tagName = (await _client.Repository.Release.GetLatest(owner, repository)).TagName;
      }
      catch (NotFoundException)
      {
        try
        {
          _logger.LogInformation($"{owner}/{repository} release is not found. Getting latest tag...");
          tagName = (await _client.Repository.GetAllTags(owner, repository)).LastOrDefault()?.Name;
        }
        catch (Exception e)
        {
          lastException = e;
        }
      }
      catch (Exception e)
      {
        lastException = e;
      }

      string errorMessage = $"{owner}/{repository} version is not found.";
      if (lastException != null)
      {
        _logger.LogError(lastException, lastException.Message);
        throw new GitHubVersionNotFoundException(errorMessage, lastException);
      }

      if (tagName == null)
      {
        throw new GitHubVersionNotFoundException(errorMessage);
      }

      _logger.LogInformation($"{owner}/{repository} latest release is {tagName}");
      return tagName;
    }

    private OctokitClient CreateGitHubClient()
    {
      var gitHubClient = new OctokitClient(new ProductHeaderValue(APP_NAME));
      string gitHubToken = _globalConfig.GitHubToken ?? Environment.GetEnvironmentVariable(ENV_GITHUB_TOKEN);
      if (gitHubToken != null)
      {
        gitHubClient.Credentials = new Credentials(gitHubToken);
      }

      return gitHubClient;
    }
  }
}