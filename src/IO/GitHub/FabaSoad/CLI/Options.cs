using CommandLine;

namespace IO.GitHub.FabaSoad.CLI
{
  public class Options
  {
    [Option('r', "repository", Required = false, HelpText = "Path to the root of a project.")]
    public string Repository { get; set; }

    [Option('u', "upgrade", Required = false, HelpText = "Upgrade verions to the latest one.")]
    public bool Upgrade { get; set; }
  }
}