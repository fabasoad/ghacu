using System;
using System.Threading.Tasks;
using Ghacu.Api.Stream;
using Ghacu.Api.Version;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Ghacu.GitHub
{
  public sealed class GitHubVersionProvider : IGitHubVersionProvider
  {
    private readonly IGitHubClient _client;
    private readonly IStreamer _streamer;

    public GitHubVersionProvider(IGitHubClient client, IStreamer streamer)
    {
      _client = client;
      _streamer = streamer;
    }

    public async Task<string> GetLatestVersionAsync(string owner, string repository)
    {
      Exception lastException = null;
      string tagName = null;
      try
      {
        _streamer.PushLine<GitHubVersionProvider>(new StreamOptions
        {
          Level = LogLevel.Debug,
          Message = $"Getting latest release for {owner}/{repository}..."
        });
        tagName = await _client.GetLatestReleaseVersionAsync(owner, repository);
      }
      catch (NotFoundException)
      {
        try
        {
          _streamer.PushLine<GitHubVersionProvider>(new StreamOptions
          {
            Level = LogLevel.Debug,
            Message = $"{owner}/{repository} release is not found. Getting latest tag..."
          });
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
        _streamer.PushLine<GitHubVersionProvider>(new StreamOptions
        {
          Color = ConsoleColor.Red,
          Level = LogLevel.Error,
          Message = lastException.Message
        });
        throw new GitHubVersionNotFoundException(errorMessage, lastException);
      }

      if (tagName == null)
      {
        throw new GitHubVersionNotFoundException(errorMessage);
      }

      _streamer.PushLine<GitHubVersionProvider>(new StreamOptions
      {
        Level = LogLevel.Debug,
        Message = $"{owner}/{repository} latest release is {tagName}"
      });
      return tagName;
    }
  }
}