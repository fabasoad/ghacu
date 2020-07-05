using System;
using System.Collections.Generic;
using System.Linq;
using Api.Entities;
using GHACU.GitHub;
using GHACU.Workflow;
using GHACU.Workflow.Entities;

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
    /// <param name="o">Parsed arguments provided by user. An instance of <see cref="Options"/> class.</param>
    public void Run(Options o)
    {
      IEnumerable<IWorkflowInfo> infos = _workflowService.GetWorkflows(o.Repository);
      IDictionary<(string, IWorkflowFile), IEnumerable<IStep>> outdated = _gitHubService.GetOutdated(o.GitHubToken, infos);
      foreach (((string name, IWorkflowFile file), IEnumerable<IStep> steps) in outdated)
      {
        Console.WriteLine($"> {name} ({file.Name})");
        var maxWidthName = 0;
        var maxWidthCurrentVersion = 0;
        var maxWidthLatestVersion = 0;
        foreach (IStep step in steps)
        {
          maxWidthName = Math.Max(maxWidthName, step.Uses.ActionName.Length);
          maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, step.Uses.CurrentVersion.ToString().Length);
          maxWidthLatestVersion = Math.Max(maxWidthLatestVersion, step.Uses.LatestVersion.ToString().Length);
        }

        foreach (IStep step in steps)
        {
          string template = "{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3," +
                            maxWidthLatestVersion + "}";
          Console.WriteLine(template, step.Uses.ActionName, step.Uses.CurrentVersion, _arrowChar,
            step.Uses.LatestVersion);
          if (o.Upgrade)
          {
            step.Upgrade(file.FilePath);
          }
        }

        Console.WriteLine();
      }

      if (!outdated.Any())
      {
        Console.WriteLine("All GitHub Actions match the latest versions.");
      }
      else if (!o.Upgrade)
      {
        Console.WriteLine("Run ghacu -u to upgrade actions.");
      }
    }
  }
}