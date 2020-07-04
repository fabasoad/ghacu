using System;
using System.Linq;
using System.Threading.Tasks;
using GHACU.Cache;
using GHACU.Workflow.Entities;
using Microsoft.Extensions.Logging;
using Octokit;

namespace GHACU.GitHub
{
    public sealed class RepositoryScanner
    {
        private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";
        private const string APP_NAME = "ghacu";
        private readonly GitHubClient _client;
        private readonly VersionsCache _cache;
        private readonly ILogger<RepositoryScanner> _logger;

        public RepositoryScanner(string gitHubToken)
        {
            _client = CreateGitHubClient(gitHubToken);
            _cache = new VersionsCache(PullLatestVersion);
            _logger = Program.LoggerFactory.CreateLogger<RepositoryScanner>();
        }

        private GitHubClient CreateGitHubClient(string gitHubToken)
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue(APP_NAME));
            gitHubToken ??= Environment.GetEnvironmentVariable(ENV_GITHUB_TOKEN);
            if (gitHubToken != null)
            {
                gitHubClient.Credentials = new Credentials(gitHubToken);
            }

            return gitHubClient;
        }

        public Task<string> GetLatestVersion(IRepositoryAware repositoryAware) => _cache.Get(repositoryAware);

        private async Task<string> PullLatestVersion(IRepositoryAware r)
        {
            Exception lastException = null;
            string tagName = null;
            try
            {
                _logger.LogInformation($"Getting latest release for {r.FullName} from GitHub...");
                tagName = (await _client.Repository.Release.GetLatest(r.Owner, r.Name)).TagName;
            }
            catch (NotFoundException)
            {
                try
                {
                    _logger.LogInformation($"{r.FullName} release is not found. Getting latest tag from GitHub...");
                    tagName = (await _client.Repository.GetAllTags(r.Owner, r.Name)).Last().Name;
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
                _logger.LogInformation($"{r.FullName} latest release is {tagName}");
            }
            else
            {
                _logger.LogError(lastException, lastException.Message);
            }

            return "N/A";
        }
    }
}