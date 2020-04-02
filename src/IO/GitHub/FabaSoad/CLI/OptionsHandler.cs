using System;
using System.IO;
using System.Linq;

using IO.GitHub.FabaSoad.GHWorkflow;

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
      foreach (var info in infos)
      {
        Console.WriteLine(info);
      }
    }
  }
}