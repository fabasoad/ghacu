using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.Api.Stream;
using Ghacu.GitHub;
using Ghacu.Runner.Cli.Print;
using Ghacu.Runner.Cli.Progress;
using Ghacu.Workflow;
using Ghacu.Workflow.Exceptions;
using Microsoft.Extensions.Logging;
using GitHubAction = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Cli
{
  /// <summary>
  /// Class to work with <see cref="Options" /> class and run.
  /// </summary>
  public sealed class CliService : ICliService, IDisposable
  {
    private readonly IGitHubService _gitHubService;
    private readonly IWorkflowService _workflowService;
    private readonly IActionPrinter _printer;
    private readonly Func<int, IProgressBar> _progressBarFactory;
    private readonly IStreamer _streamer;
    private IProgressBar _progressBar;

    public CliService(
      IWorkflowService workflowService,
      IGitHubService gitHubService,
      IActionPrinter printer,
      Func<int, IProgressBar> progressBarFactory,
      IStreamer streamer)
    {
      _workflowService = workflowService;
      _gitHubService = gitHubService;
      _printer = printer;
      _progressBarFactory = progressBarFactory;
      _streamer = streamer;
      _gitHubService.RepositoryChecked += ProgressBarProcessed;
      _gitHubService.RepositoryCheckedStarted += ProgressBarPrepare;
      _gitHubService.RepositoryCheckedFinished += ProgressBarDispose;
    }

    private void ProgressBarPrepare(int totalTicks) => _progressBar = _progressBarFactory(totalTicks);
    
    private void ProgressBarProcessed(RepositoryCheckedArgs args) => _progressBar.Report(args.ProgressValue);

    private void ProgressBarDispose() => _progressBar.Dispose();

    /// <summary>
    /// Run GHACU logic on repository provided by user.
    /// </summary>
    /// <param name="repository">Path to repository.</param>
    /// <param name="shouldUpgrade">If true, actions will be updated, otherwise - just checking will be performed.</param>
    public void Run(string repository, bool shouldUpgrade)
    {
      IEnumerable<WorkflowInfo> infos;
      try
      {
        infos = _workflowService.GetWorkflows(repository);
      }
      catch (WorkflowValidationException e)
      {
        _streamer.Push<CliService>(new StreamOptions
        {
          Exception = e,
          Level = LogLevel.Error,
          Messages = new StreamMessageBuilder().Add(e.Message, ConsoleColor.Red).Build()
        });
        return;
      }

      IDictionary<WorkflowInfo, IEnumerable<GitHubAction>> outdated = _gitHubService.GetOutdated(infos);
      foreach ((WorkflowInfo wfi, IEnumerable<GitHubAction> actions) in outdated)
      {
        _printer.PrintHeader(wfi.Workflow.Name, wfi.File.Name);
        _printer.Print(actions);

        if (shouldUpgrade)
        {
          wfi.Upgrade();
        }
      }

      if (!outdated.Any())
      {
        _printer.PrintNoUpgradeNeeded();
      }
      else if (!shouldUpgrade)
      {
        _streamer.PushEmpty();
        _printer.PrintRunUpgrade();
      }
    }

    public void Dispose()
    {
      _gitHubService.RepositoryChecked -= ProgressBarProcessed;
      _gitHubService.RepositoryCheckedStarted -= ProgressBarPrepare;
      _gitHubService.RepositoryCheckedFinished -= ProgressBarDispose;
      _progressBar?.Dispose();
    }
  }
}