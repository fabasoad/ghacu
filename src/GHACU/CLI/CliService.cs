using System;
using System.Collections.Generic;
using System.Linq;
using Ghacu.GitHub;
using Ghacu.Api.Entities;
using Ghacu.Workflow;

namespace GHACU.CLI
{
  /// <summary>
  /// Class to work with <see cref="Options"/> class and run 
  /// </summary>
  public sealed class CliService : ICliService
  {
    private readonly char _arrowChar = Convert.ToChar(187);
    private readonly IWorkflowService _workflowService;
    private readonly IGitHubService _gitHubService;

    public CliService(IWorkflowService workflowService, IGitHubService gitHubService)
    {
      _workflowService = workflowService;
      _gitHubService = gitHubService;
    }

    /// <summary>
    /// Run GHACU logic based on CLI arguments that were provided by user.
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="shouldUpgrade"></param>
    public void Run(string repository, bool shouldUpgrade)
    {
      IEnumerable<WorkflowInfo> infos = _workflowService.GetWorkflows(repository);
      IDictionary<WorkflowInfo, IEnumerable<Step>> outdated = _gitHubService.GetOutdated(infos);
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
        Console.WriteLine("All GitHub Actions match the latest versions.");
      }
      else if (!shouldUpgrade)
      {
        Console.WriteLine("Run ghacu -u to upgrade actions.");
      }
    }
  }
}