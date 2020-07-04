using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GHACU.Workflow;
using GHACU.Workflow.Analyze;
using Microsoft.Extensions.Logging;

namespace GHACU.CLI
{
  public sealed class OptionsHandler
  {
    private const string GITHUB_FOLDER = ".github";
    private const string WORKFLOWS_FOLDER = "workflows";
    private readonly char _arrowChar = Convert.ToChar(187);
    private readonly ILogger<OptionsHandler> _logger;

    public OptionsHandler()
    {
      _logger = Program.LoggerFactory.CreateLogger<OptionsHandler>();
    }

    public void Handle(Options o)
    {
      string wfPath;
      try
      {
        wfPath = BuildWorkflowPath(o.Repository);
      }
      catch (OptionValidationException e)
      {
        _logger.LogError(e, e.Message);
        return;
      }

      var parser = new WorkflowParser();
      IEnumerable<WorkflowInfo> infos = parser.Parse(new[] { "*.yml", "*.yaml" }
        .SelectMany(p => Directory.EnumerateFiles(wfPath, p, SearchOption.AllDirectories)));

      var analyzer = new WorkflowAnalyzer(o.GitHubToken);
      IEnumerable<WorkflowAnalyzerResult> results = analyzer.GetOutdated(infos);

      foreach (var r in results)
      {
        Console.WriteLine($"> {r.Name} ({r.File})");
        int maxWidthName = 0;
        int maxWidthCurrentVersion = 0;
        int maxWidthLatestVersion = 0;
        foreach (var a in r.Actions)
        {
          maxWidthName = Math.Max(maxWidthName, a.Name.Length);
          maxWidthCurrentVersion = Math.Max(maxWidthCurrentVersion, a.CurrentVersion.Length);
          maxWidthLatestVersion = Math.Max(maxWidthLatestVersion, a.LatestVersion.Length);
        }

        foreach (var a in r.Actions)
        {
          var template = "{0,-" + maxWidthName + "}  {1," + maxWidthCurrentVersion + "}  {2}  {3," +
                         maxWidthLatestVersion + "}";
          Console.WriteLine(template, a.Name, a.CurrentVersion, _arrowChar, a.LatestVersion);
        }

        Console.WriteLine();
        if (o.Upgrade)
        {
          r.Upgrade();
        }
      }

      if (!results.Any())
      {
        Console.WriteLine("All GitHub Actions match the latest versions.");
      }
      else if (!o.Upgrade)
      {
        Console.WriteLine("Run ghacu -u to upgrade actions.");
      }
    }

    private string BuildWorkflowPath(string repository)
    {
      string rep;
      if (string.IsNullOrWhiteSpace(repository))
      {
        rep = Directory.GetCurrentDirectory();
      }
      else
      {
        if (!Directory.Exists(repository))
        {
          throw new OptionValidationException($"Directory {repository} does not exist.");
        }
        else
        {
          rep = repository;
        }
      }

      string ghPath = Path.Combine(rep, GITHUB_FOLDER);
      if (!Directory.Exists(ghPath))
      {
        throw new OptionValidationException($"Directory {GITHUB_FOLDER} does not exist. Nothing to check.");
      }

      string wfPath = Path.Combine(ghPath, WORKFLOWS_FOLDER);
      if (!Directory.Exists(wfPath))
      {
        throw new OptionValidationException(
          $"Directory {Path.Combine(GITHUB_FOLDER, WORKFLOWS_FOLDER)} does not exist. Nothing to check.");
      }

      return wfPath;
    }
  }
}