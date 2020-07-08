using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.GitHub;
using Ghacu.GitHub.Exceptions;
using Ghacu.Workflow;
using Ghacu.Workflow.Exceptions;

namespace Ghacu.Runner.Cli
{
  /// <summary>
  ///   Class to work with <see cref="Options" /> class and run.
  /// </summary>
  public sealed class CliService : ICliService
  {
    private readonly char _arrowChar = Convert.ToChar(187);
    private readonly IGitHubService _gitHubService;
    private readonly IWorkflowService _workflowService;

    public CliService(IWorkflowService workflowService, IGitHubService gitHubService)
    {
      _workflowService = workflowService;
      _gitHubService = gitHubService;
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
        Console.WriteLine($"> {wfi.Workflow.Name} ({wfi.File.Name})");
        var maxWidthName = 0;
        var maxWidthCurrentVersion = 0;
        var maxWidthLatestVersion = 0;
        foreach (Step step in steps)
        {
          maxWidthName = Math.Max(maxWidthName, step.Uses.ActionName.Length);
          maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, step.Uses.CurrentVersion.Value.Length);
          maxWidthLatestVersion = Math.Max(maxWidthLatestVersion, step.Uses.GetLatestVersion().Value.Length);
        }

        foreach (Step step in steps)
        {
          string template = "{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3," +
                            maxWidthLatestVersion + "}";
          Console.WriteLine(template, step.Uses.ActionName, step.Uses.CurrentVersion.Value, _arrowChar,
            step.Uses.GetLatestVersion().Value);
        }

        if (shouldUpgrade)
        {
          wfi.Upgrade();
        }

        Console.WriteLine();
      }

      if (!outdated.Any())
      {
        Console.Write("All GitHub Actions match the latest versions.");
      }
      else if (!shouldUpgrade)
      {
        Console.Write("Run ghacu -u to upgrade actions.");
      }
    }
  }
}