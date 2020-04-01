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
      if (string.IsNullOrWhiteSpace(o.Repository))
      {
        Console.WriteLine("Please define path to the repository");
      }
      else if (!Directory.Exists(o.Repository))
      {
        Console.WriteLine($"Directory {o.Repository} does not exist.");
      }
      else
      {
        var ghPath = Path.Combine(o.Repository, GITHUB_FOLDER);
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
        foreach (var file in files)
        {
          Console.WriteLine(parser.Parse(file));
        }
      }
    }
  }
}