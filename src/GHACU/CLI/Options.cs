using CommandLine;

namespace GHACU.CLI
{
  public sealed class Options
  {
    [Option('r', "repository", Required = false, HelpText = "Path to the root of a project.")]
    public string Repository { get; set; }

    [Option('u', "upgrade", Required = false, HelpText = "Upgrade versions to the latest one.")]
    public bool Upgrade { get; set; }

    [Option('t', "token", Required = false, HelpText = "GitHub token to work with actions repositories.")]
    public string GitHubToken { get; set; }
  }
}