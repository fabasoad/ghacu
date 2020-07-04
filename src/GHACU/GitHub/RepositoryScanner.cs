using System;
using System.Linq;
using System.Threading.Tasks;
using GHACU.Cache;
using GHACU.PubSub;
using GHACU.Workflow.Entities;
using Octokit;
using PubSub;

namespace GHACU.GitHub
{
    public sealed class RepositoryScanner
    {
        private const string ENV_GITHUB_TOKEN = "GHACU_GITHUB_TOKEN";
        private const string APP_NAME = "ghacu";
        private readonly GitHubClient _client;
        private readonly VersionsCache _cache;
        private readonly Hub _hub;

        public RepositoryScanner(string gitHubToken)
        {
            _hub = Hub.Default;
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
            Exception lastException = null;
            string tagName = null;
            try
            {
                _hub.Publish(new PubSubMessage
                {
                    Action = PubSubAction.STARTED,
                    Topic = PubSubTopic.GITHUB,
                    Message = $"Getting latest release for {r.FullName}..."
                });
                tagName = (await _client.Repository.Release.GetLatest(r.Owner, r.Name)).TagName;
            }
            catch (NotFoundException)
            {
                try
                {
                    _hub.Publish(new PubSubMessage
                    {
                        Action = PubSubAction.STARTED,
                        Topic = PubSubTopic.GITHUB,
                        Message = $"{r.FullName} release is not found. Getting latest tag..."
                    });
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
                _hub.Publish(new PubSubMessage
                {
                    Action = PubSubAction.SUCCEED,
                    Topic = PubSubTopic.GITHUB,
                    Message = $"{r.FullName} latest release is {tagName}"
                });
            }
            else
            {
                Console.WriteLine(lastException.Message);
            }

            return "N/A";
        }
    }
}