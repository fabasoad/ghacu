using System;
using System.Linq;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Ghacu.GitHub
{
  public sealed class GitHubVersionProvider : ILatestVersionProvider
  {
    private readonly ILogger<GitHubVersionProvider> _logger;
    private readonly IGitHubClient _client;

    public GitHubVersionProvider(ILoggerFactory loggerFactory, IGitHubClient client)
    {
      _logger = loggerFactory.CreateLogger<GitHubVersionProvider>();
      _client = client;
    }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      Exception lastException = null;
      string tagName = null;
      try
      {
        _logger.LogInformation($"Getting latest release for {owner}/{repository}...");
        tagName = await _client.GetLatestReleaseVersionAsync(owner, repository);
      }
      catch (NotFoundException)
      {
        try
        {
          _logger.LogInformation($"{owner}/{repository} release is not found. Getting latest tag...");
          tagName = await _client.GetLatestTagVersionAsync(owner, repository);
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
  }
}