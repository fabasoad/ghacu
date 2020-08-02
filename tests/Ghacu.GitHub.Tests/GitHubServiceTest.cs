using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ghacu.Api;
using Ghacu.Api.Entities;
using Ghacu.Api.Stream;
using Ghacu.Api.Version;
using Ghacu.GitHub.Exceptions;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using Xunit;
using Action = Ghacu.Api.Entities.Action;

namespace Ghacu.GitHub.Tests
{
  public class GitHubServiceTest
  {
    [Fact]
    public void GetOutdated_Positive()
    {
      const int expectedTotalCount = 3;
      const string owner1 = "own1";
      const string repository1 = "repo1";
      const string latestVersion1 = "v0.0.1-alpha.3+3465";
      const string owner2 = "own2";
      const string repository2 = "repo2";
      const string currentVersion2 = "v1.0.0";
      const string latestVersion2 = "v1.0.1";
      const string owner3 = "own3";
      const string repository3 = "repo3";
      const string latestVersion3 = "v1";
      
      var providerMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => providerMock.GetLatestVersionAsync(owner1, repository1))
        .Returns(Task.FromResult(latestVersion1));
      Mock.Arrange(() => providerMock.GetLatestVersionAsync(owner2, repository2))
        .Returns(Task.FromResult(latestVersion2));
      Mock.Arrange(() => providerMock.GetLatestVersionAsync(owner3, repository3))
        .Returns(Task.FromResult(latestVersion3));

      var semaphore = new SemaphoreSlim(1, 1);
      var semaphoreProxyMock = Mock.Create<ISemaphoreSlimProxy>();
      Mock.Arrange(() => semaphoreProxyMock.WaitAsync())
        .Returns(() => semaphore.WaitAsync()).Occurs(expectedTotalCount);
      Mock.Arrange(() => semaphoreProxyMock.Release())
        .Returns(() => semaphore.Release()).Occurs(expectedTotalCount);
      
      var streamerMock = Mock.Create<IStreamer>();

      var gitHubService = new GitHubService(providerMock, semaphoreProxyMock, streamerMock);
      
      var repositoryChecked = 0;
      gitHubService.RepositoryChecked += args =>
      {
        ++repositoryChecked;
        Assert.Equal((double)repositoryChecked / expectedTotalCount, args.ProgressValue);
      };
      var repositoryCheckedStarted = 0;
      gitHubService.RepositoryCheckedStarted += actualTotalCount =>
      {
        repositoryCheckedStarted++;
        Assert.Equal(expectedTotalCount, actualTotalCount);
      };
      var repositoryCheckedFinished = 0;
      gitHubService.RepositoryCheckedFinished += () =>
      {
        repositoryCheckedFinished++;
      };
      var workflowInfos = new List<WorkflowInfo>
      {
        new WorkflowInfo(default, new ActionWorkflow
        {
          Jobs = new Dictionary<string, Job>
          {
            {
              "job-1",
              new Job
              {
                Steps = new List<Step>
                {
                  new Step { Uses = $"docker://{owner1}/{repository1}:v0.0.1-alpha.3+3465" },
                  new Step { Uses = $"{owner2}/{repository2}@{currentVersion2}" },
                  new Step { Uses = "./" },
                }
              }
            },
            {
              "job-2",
              new Job
              {
                Steps = new List<Step>
                {
                  new Step { Uses = $"docker://{owner3}/{repository3}:v2" },
                  new Step { Uses = null },
                }
              }
            }
          }
        })
      };
      IDictionary<WorkflowInfo, IEnumerable<Action>> actual = gitHubService.GetOutdated(workflowInfos);
      Assert.Single(actual);
      Assert.True(actual.ContainsKey(workflowInfos[0]));
      IEnumerable<Action> actions = actual[workflowInfos[0]];
      Assert.Single(actions);
      Action action = actions.Single();
      Assert.Equal(owner2, action.Owner);
      Assert.Equal(repository2, action.Repository);
      Assert.Equal(currentVersion2, action.CurrentVersion);
      Assert.Equal(latestVersion2, action.LatestVersion);

      Assert.Equal(1, repositoryCheckedStarted);
      Assert.Equal(expectedTotalCount, repositoryChecked);
      Assert.Equal(1, repositoryCheckedFinished);
      
      Mock.Assert(providerMock);
      Mock.Assert(semaphoreProxyMock);
      Mock.Assert(streamerMock);
    }

    [Theory]
    [MemberData(nameof(DataGetOutdatedGetLatestVersionAsyncThrows))]
    public void GetOutdated_GetLatestVersionAsync_Throws(
      Exception expectedException, LogLevel expectedLevel, ConsoleColor expectedColor)
    {
      const int expectedTotalCount = 1;
      const string owner1 = "own1";
      const string repository1 = "repo1";
      
      var providerMock = Mock.Create<ILatestVersionProvider>();
      Mock.Arrange(() => providerMock.GetLatestVersionAsync(owner1, repository1))
        .Throws(expectedException);

      var semaphoreProxyMock = Mock.Create<ISemaphoreSlimProxy>();
      Mock.Arrange(() => semaphoreProxyMock.WaitAsync())
        .Returns(Task.CompletedTask).Occurs(expectedTotalCount);
      Mock.Arrange(() => semaphoreProxyMock.Release())
        .Returns(1 /* any int */).Occurs(expectedTotalCount);
      
      var streamerMock = Mock.Create<IStreamer>();
      Mock.Arrange(() => streamerMock.PushLine<GitHubService>(
        Arg.Matches<StreamOptions>(options =>
          ReferenceEquals(expectedException, options.Exception)
          && options.Level == expectedLevel
          && options.Messages.Count() == 1
          && options.Messages.Single().Color == expectedColor
          && options.Messages.Single().Message == expectedException.Message)))
        .Occurs(expectedTotalCount);

      var gitHubService = new GitHubService(providerMock, semaphoreProxyMock, streamerMock);
      
      var repositoryChecked = 0;
      gitHubService.RepositoryChecked += args =>
      {
        ++repositoryChecked;
        Assert.Equal((double)repositoryChecked / expectedTotalCount, args.ProgressValue);
      };
      var repositoryCheckedStarted = 0;
      gitHubService.RepositoryCheckedStarted += actualTotalCount =>
      {
        repositoryCheckedStarted++;
        Assert.Equal(expectedTotalCount, actualTotalCount);
      };
      var repositoryCheckedFinished = 0;
      gitHubService.RepositoryCheckedFinished += () =>
      {
        repositoryCheckedFinished++;
      };
      var workflowInfos = new List<WorkflowInfo>
      {
        new WorkflowInfo(default, new ActionWorkflow
        {
          Jobs = new Dictionary<string, Job>
          {
            {
              "job-1",
              new Job
              {
                Steps = new List<Step>
                {
                  new Step { Uses = $"docker://{owner1}/{repository1}:v0.0.1-alpha.3+3465" },
                  new Step { Uses = "./" },
                }
              }
            },
            {
              "job-2",
              new Job
              {
                Steps = new List<Step>
                {
                  new Step { Uses = null },
                }
              }
            }
          }
        })
      };
      IDictionary<WorkflowInfo, IEnumerable<Action>> actual = gitHubService.GetOutdated(workflowInfos);
      Assert.Empty(actual);

      Assert.Equal(1, repositoryCheckedStarted);
      Assert.Equal(expectedTotalCount, repositoryChecked);
      Assert.Equal(1, repositoryCheckedFinished);
      
      Mock.Assert(providerMock);
      Mock.Assert(semaphoreProxyMock);
      Mock.Assert(streamerMock);
    }

    public static IEnumerable<object[]> DataGetOutdatedGetLatestVersionAsyncThrows => new List<object[]>
    {
      new object[]
      {
        new GitHubVersionNotFoundException("test-exception-message"), LogLevel.Warning, ConsoleColor.Yellow
      },
      new object[] { new Exception("test-exception-message"), LogLevel.Error, ConsoleColor.Red }
    };
  }
}