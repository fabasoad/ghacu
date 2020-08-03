using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.Api.Stream;
using Ghacu.GitHub;
using Ghacu.Runner.Cli;
using Ghacu.Runner.Cli.Print;
using Ghacu.Runner.Cli.Progress;
using Ghacu.Workflow;
using Ghacu.Workflow.Exceptions;
using Microsoft.Extensions.Logging;
using Telerik.JustMock;
using Telerik.JustMock.Expectations.Abstraction;
using Xunit;
using GitHubAction = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Tests.Cli
{
  public class CliServiceTest
  {
    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(false, false)]
    public void Run_ShouldUpgrade(bool shouldUpgrade, bool isOutdatedEmpty)
    {
      const string repository = "test-repository";

      IEnumerable<WorkflowInfo> infos = new List<WorkflowInfo>();
      var workflowServiceMock = Mock.Create<IWorkflowService>();
      Mock.Arrange(() => workflowServiceMock.GetWorkflows(repository)).Returns(infos);

      IDictionary<WorkflowInfo, IEnumerable<GitHubAction>> outdated;
      var actionPrinterMock = Mock.Create<IActionPrinter>();
      if (isOutdatedEmpty)
      {
        outdated = new Dictionary<WorkflowInfo, IEnumerable<GitHubAction>>();
        Mock.Arrange(() => actionPrinterMock.PrintHeader(Arg.AnyString, Arg.AnyString))
          .OccursNever();
        Mock.Arrange(() => actionPrinterMock.Print(Arg.IsAny<IEnumerable<GitHubAction>>()))
          .OccursNever();
        Mock.Arrange(() => actionPrinterMock.PrintNoUpgradeNeeded()).DoNothing().OccursOnce();
      }
      else
      {
        const string fileName = ".github/test-file";
        var actionWorkflow = new ActionWorkflow { Name = "test-action-workflow-name" };
        var actions = new List<GitHubAction>();
        outdated = new Dictionary<WorkflowInfo, IEnumerable<GitHubAction>>
        {
          { new WorkflowInfo($"some-folder/{fileName}", actionWorkflow), actions }
        };
        Mock.Arrange(() => actionPrinterMock.PrintHeader(actionWorkflow.Name, fileName))
          .DoNothing().OccursOnce();
        Mock.Arrange(() => actionPrinterMock.Print(actions))
          .DoNothing().OccursOnce();
        Mock.Arrange(() => actionPrinterMock.PrintNoUpgradeNeeded()).OccursNever();
      }

      var gitHubServiceMock = Mock.Create<IGitHubService>();
      Mock.Arrange(() => gitHubServiceMock.GetOutdated(infos)).Returns(outdated);
      
      var progressBarMock = Mock.Create<Ghacu.Runner.Cli.Progress.IProgressBar>();
      Mock.Arrange(() => progressBarMock.Report(0.25)).DoNothing().OccursOnce();
      Mock.Arrange(() => progressBarMock.Dispose()).DoNothing().OccursOnce();

      var streamerMock = Mock.Create<IStreamer>();
      Mock.Arrange(() => streamerMock.Push<CliService>(Arg.IsAny<StreamOptions>())).OccursNever();

      IAssertable pushEmptySetup = Mock.Arrange(() => streamerMock.PushEmpty()).DoNothing();
      IAssertable runUpgradeSetup = Mock.Arrange(() => actionPrinterMock.PrintRunUpgrade()).DoNothing();
      if (isOutdatedEmpty)
      {
        pushEmptySetup.OccursNever();
        runUpgradeSetup.OccursNever();
      }
      else if (!shouldUpgrade)
      {
        pushEmptySetup.OccursOnce();
        runUpgradeSetup.OccursOnce();
      }
      
      var service = new CliService(
        workflowServiceMock, gitHubServiceMock, actionPrinterMock, null, streamerMock);
      service.Run(repository, shouldUpgrade);
      
      Mock.Assert(workflowServiceMock);
      Mock.Assert(gitHubServiceMock);
      Mock.Assert(actionPrinterMock);
      Mock.Assert(streamerMock);
    }

    [Theory]
    [MemberData(nameof(DataRunGetWorkflowsError))]
    public void Run_GetWorkflows_Error(
      Exception expectedException, LogLevel expectedLogLevel, ConsoleColor expectedColor)
    {
      const string repository = "test-repository";
      var workflowServiceMock = Mock.Create<IWorkflowService>();
      Mock.Arrange(() => workflowServiceMock.GetWorkflows(repository)).Throws(expectedException);
      var gitHubServiceMock = Mock.Create<IGitHubService>();
      Mock.Arrange(() => gitHubServiceMock.GetOutdated(Arg.IsAny<IEnumerable<WorkflowInfo>>()));
      var actionPrinterMock = Mock.Create<IActionPrinter>();
      Mock.Arrange(() => actionPrinterMock.PrintHeader(Arg.AnyString, Arg.AnyString))
        .OccursNever();
      Mock.Arrange(() => actionPrinterMock.Print(Arg.IsAny<IEnumerable<GitHubAction>>())).OccursNever();
      Mock.Arrange(() => actionPrinterMock.PrintNoUpgradeNeeded()).OccursNever();
      Mock.Arrange(() => actionPrinterMock.PrintRunUpgrade()).OccursNever();
      var streamerMock = Mock.Create<IStreamer>();
      Mock.Arrange(() => streamerMock.Push<CliService>(Arg.Matches<StreamOptions>(options =>
        ReferenceEquals(expectedException, options.Exception)
          && options.Level == expectedLogLevel
          && options.Messages.Count() == 1
          && options.Messages.Single().Color == expectedColor
          && options.Messages.Single().Message == expectedException.Message)))
        .DoNothing()
        .OccursOnce();
      Mock.Arrange(() => streamerMock.PushEmpty()).OccursNever();
      var service = new CliService(
        workflowServiceMock, gitHubServiceMock, actionPrinterMock, null, streamerMock);
      service.Run(repository, false /* any boolean */);
      
      Mock.Assert(workflowServiceMock);
      Mock.Assert(gitHubServiceMock);
      Mock.Assert(actionPrinterMock);
      Mock.Assert(streamerMock);
    }
    
    public static IEnumerable<object[]> DataRunGetWorkflowsError => new List<object[]>
    {
      new object[] { new WorkflowValidationException("exception-message"), LogLevel.Error, ConsoleColor.Red },
      new object[] { new Exception("exception-message"), LogLevel.Critical, ConsoleColor.DarkRed }
    };

    [Fact]
    public void Dispose_Positive_WithProgressBar()
    {
      const int index = 1;
      const int expectedTotalTicks = 5;
      var gitHubServiceMock = Mock.Create<IGitHubService>();

      var progressBarMock = Mock.Create<IProgressBar>();
      Mock.Arrange(() => progressBarMock.Report((double)index / expectedTotalTicks)).DoNothing().OccursOnce();
      Mock.Arrange(() => progressBarMock.Dispose()).DoNothing().Occurs(2);

      var createProgressBarTotalCalls = 0;
      IProgressBar CreateProgressBar(int actualTotalTicks)
      {
        createProgressBarTotalCalls++;
        Assert.Equal(expectedTotalTicks, actualTotalTicks);
        return progressBarMock;
      }
      
      var service = new CliService(
        null, gitHubServiceMock, null, CreateProgressBar, null);
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedStarted += null, expectedTotalTicks);
      Mock.Raise(() => gitHubServiceMock.RepositoryChecked += null, new RepositoryCheckedArgs(index, expectedTotalTicks));
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedFinished += null);
      service.Dispose();
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedStarted += null, expectedTotalTicks);
      Mock.Raise(() => gitHubServiceMock.RepositoryChecked += null, new RepositoryCheckedArgs(index, expectedTotalTicks));
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedFinished += null);
      
      Assert.Equal(1, createProgressBarTotalCalls);
      Mock.Assert(progressBarMock);
    }

    [Theory]
    [MemberData(nameof(DataDisposePositiveWithoutProgressBar))]
    public void Dispose_Positive_WithoutProgressBar(
      Func<int, int[], Func<int, IProgressBar>> factoryOfProgressBarFactory, int expectedNumCalls)
    {
      const int index = 1;
      const int expectedTotalTicks = 5;
      var gitHubServiceMock = Mock.Create<IGitHubService>();
      var actualNumCalls = new[] { 0 };
      var service = new CliService(
        null,
        gitHubServiceMock,
        null,
        factoryOfProgressBarFactory(expectedTotalTicks, actualNumCalls),
        null);
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedStarted += null, expectedTotalTicks);
      Mock.Raise(() => gitHubServiceMock.RepositoryChecked += null, new RepositoryCheckedArgs(index, expectedTotalTicks));
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedFinished += null);
      service.Dispose();
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedStarted += null, expectedTotalTicks);
      Mock.Raise(() => gitHubServiceMock.RepositoryChecked += null, new RepositoryCheckedArgs(index, expectedTotalTicks));
      Mock.Raise(() => gitHubServiceMock.RepositoryCheckedFinished += null);
      
      Assert.Equal(expectedNumCalls, actualNumCalls[0]);
    }
    
    public static IEnumerable<object[]> DataDisposePositiveWithoutProgressBar => new List<object[]>
    {
      new object[]
      {
        new Func<int, int[], Func<int, IProgressBar>>((expected, numCalls) => actual =>
        {
          numCalls[0]++;
          Assert.Equal(expected, actual);
          return null;
        }), 1
      },
      new object[] { new Func<int, int[], Func<int, IProgressBar>>((_1, _2) => null), 0 }
    };
  } 
}