using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octokit;
using OctokitClient = Octokit.GitHubClient;

namespace GHACU.GitHub
{
    public sealed class GitHubClient : IGitHubClient
    {
        private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";
        private const string APP_NAME = "ghacu";
        private OctokitClient _client;
        private readonly ILogger<GitHubClient> _logger;
        private string _token;

        public GitHubClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GitHubClient>();
        }

        private OctokitClient CreateGitHubClient(string gitHubToken)
        {
            var gitHubClient = new OctokitClient(new ProductHeaderValue(APP_NAME));
            gitHubToken ??= Environment.GetEnvironmentVariable(ENV_GITHUB_TOKEN);
            if (gitHubToken != null)
            {
                gitHubClient.Credentials = new Credentials(gitHubToken);
            }

            return gitHubClient;
        }

        public IGitHubClient WithToken(string token)
        {
            _token = token;
            return this;
        }

        public async Task<string> GetLatestRelease(string owner, string repository)
        {
            _client ??= CreateGitHubClient(_token);
            Exception lastException = null;
            string tagName = null;
            try
            {
                _logger.LogInformation($"Getting latest release for {owner}/{repository} from GitHub...");
                tagName = (await _client.Repository.Release.GetLatest(owner, repository)).TagName;
            }
            catch (NotFoundException)
            {
                try
                {
                    _logger.LogInformation($"{owner}/{repository} release is not found. Getting latest tag from GitHub...");
                    tagName = (await _client.Repository.GetAllTags(owner, repository)).Last().Name;
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

            if (lastException == null)
            {
                _logger.LogInformation($"{owner}/{repository} latest release is {tagName}");
            }
            else
            {
                _logger.LogError(lastException, lastException.Message);
            }

            return "N/A";
        }
    }
}