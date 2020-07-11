using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.GitHub;
using Ghacu.GitHub.Exceptions;
using Ghacu.Runner.Cli.Print;
using Ghacu.Workflow;
using Ghacu.Workflow.Exceptions;

namespace Ghacu.Runner.Cli
{
  /// <summary>
  ///   Class to work with <see cref="Options" /> class and run.
  /// </summary>
  public sealed class CliService : ICliService
  {
    private readonly IGitHubService _gitHubService;
    private readonly IWorkflowService _workflowService;
    private readonly IActionPrinter _printer;

    public CliService(
      IWorkflowService workflowService, IGitHubService gitHubService, IActionPrinter printer)
    {
      _workflowService = workflowService;
      _gitHubService = gitHubService;
      _printer = printer;
    }

    /// <summary>
    ///   Run GHACU logic on repository provided by user.
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
        Console.Write(e.Message);
        return;
      }

      IDictionary<WorkflowInfo, IEnumerable<Step>> outdated;
      try
      {
        outdated = _gitHubService.GetOutdated(infos);
      }
      catch (GitHubVersionNotFoundException e)
      {
        Console.Write(e.Message);
        return;
      }

      foreach ((WorkflowInfo wfi, IEnumerable<Step> steps) in outdated)
      {
        _printer.PrintHeader(wfi.Workflow.Name, wfi.File.Name);
        _printer.Print(steps);

        if (shouldUpgrade)
        {
          wfi.Upgrade();
        }

        Console.WriteLine();
      }

      if (!outdated.Any())
      {
        _printer.PrintNoUpgradeNeeded();
      }
      else if (!shouldUpgrade)
      {
        _printer.PrintRunUpgrade();
      }
    }
  }
}