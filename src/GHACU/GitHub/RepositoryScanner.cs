using System;
using System.Linq;
using System.Threading.Tasks;
using GHACU.Cache;
using GHACU.Workflow.Entities;
using Octokit;

namespace GHACU.GitHub
{
    public sealed class RepositoryScanner
    {
        private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";
        private const string APP_NAME = "ghacu";
        private readonly GitHubClient _client;
        private readonly VersionsCache _cache;

        public RepositoryScanner(string gitHubToken)
        {
            _client = CreateGitHubClient(gitHubToken);
            _cache = new VersionsCache(PullLatestVersion);
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

        public async Task<string> GetLatestVersion(IRepositoryAware repositoryAware) =>
            await _cache.Get(repositoryAware);

        private async Task<string> PullLatestVersion(IRepositoryAware r)
        {
            Exception lastException;
            try
            {
                return (await _client.Repository.Release.GetLatest(r.Owner, r.Name)).TagName;
            }
            catch (NotFoundException)
            {
                try
                {
                    return (await _client.Repository.GetAllTags(r.Owner, r.Name)).Last().Name;
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

            if (lastException != null)
            {
                Console.WriteLine(lastException.Message);
            }

            return "N/A";
        }
    }
}