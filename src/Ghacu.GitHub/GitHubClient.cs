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
    private OctokitClient _client;
    private readonly ILogger<GitHubClient> _logger;
    private readonly IGlobalConfig _globalConfig;

    public GitHubClient(ILoggerFactory loggerFactory, IGlobalConfig globalConfig)
    {
      _logger = loggerFactory.CreateLogger<GitHubClient>();
      _globalConfig = globalConfig;
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

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      _client ??= CreateGitHubClient();
      string tagName;
      try
      {
        _logger.LogInformation($"Getting latest release for {owner}/{repository}Ghacu.GitHubGitHub...");
        tagName = (await _client.Repository.Release.GetLatest(owner, repository)).TagName;
      }
      catch (NotFoundException)
      {
        try
        {
          _logger.LogInformation($"{owner}/{repository} release is not found. Getting latest tagGhacu.GitHubGitHub...");
          tagName = (await _client.Repository.GetAllTags(owner, repository)).Last().Name;
        }
        catch (Exception e)
        {
          throw new GitHubVersionNotFoundException(e.Message, e);
        }
      }
      catch (Exception e)
      {
        throw new GitHubVersionNotFoundException(e.Message, e);
      }

      _logger.LogInformation($"{owner}/{repository} latest release is {tagName}");
      return tagName;
    }
  }
}