using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IO.GitHub.FabaSoad.GHWorkflow;
using IO.GitHub.FabaSoad.GHWorkflow.Analyze;

namespace IO.GitHub.FabaSoad.CLI
{
  public class OptionsHandler
  {
    private const String GITHUB_FOLDER = ".github";
    private const String WORKFLOWS_FOLDER = "workflows";

    public void Handle(Options o)
    {
      string repository;
      if (string.IsNullOrWhiteSpace(o.Repository))
      {
        repository = Directory.GetCurrentDirectory();
      }
      else
      {
        if (!Directory.Exists(o.Repository))
        {
          Console.WriteLine($"Directory {o.Repository} does not exist.");
          return;
        }
        else
        {
          repository = o.Repository;
        }
      }
      var ghPath = Path.Combine(repository, GITHUB_FOLDER);
      if (!Directory.Exists(ghPath))
      {
        Console.WriteLine($"Directory {GITHUB_FOLDER} does not exist. Nothing to check.");
        return;
      }
      var wfPath = Path.Combine(ghPath, WORKFLOWS_FOLDER);
      if (!Directory.Exists(wfPath))
      {
        Console.WriteLine($"Directory {Path.Combine(GITHUB_FOLDER, WORKFLOWS_FOLDER)} does not exist. Nothing to check.");
        return;
      }
      var files = new[] { "*.yml", "*.yaml" }.SelectMany(p => Directory.EnumerateFiles(wfPath, p, SearchOption.AllDirectories));
      
      var parser = new WorkflowParser();
      var infos = parser.Parse(files);
      
      var analyzer = new WorkflowAnalyzer();
      var results = analyzer.Analyze(infos).GetAwaiter().GetResult();
      
      foreach (var r in results)
      {
        Console.WriteLine($"> {r.Name} ({r.File})");
        var widthName = r.Actions.Where(a => !a.IsUpToDate).Select(a => a.Name.Length).Max();
        var widthCurrentVersion = r.Actions.Where(a => !a.IsUpToDate).Select(a => a.CurrentVersion.Length).Max();
        var widthLatestVersion = r.Actions.Where(a => !a.IsUpToDate).Select(a => a.LatestVersion.Length).Max();
        foreach (var a in r.Actions.Where(a => !a.IsUpToDate))
        {
          var template = "{0,-" + widthName + "}  {1," + widthCurrentVersion + "}  {2}  {3," + widthLatestVersion + "}";
          Console.WriteLine(string.Format(template, a.Name, a.CurrentVersion, Convert.ToChar(187), a.LatestVersion));
        }
        Console.WriteLine();
      }
    }
  }
}