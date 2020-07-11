using CommandLine;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli
{
  public sealed class Options
  {
    [Option("cache", Required = false, HelpText = "Enable cache.", Default = BooleanOption.Yes)]
    public BooleanOption UseCache { get; set; }
    
    [Option("color", Required = false, HelpText = "Enable colors in console output.", Default = BooleanOption.Yes)]
    public BooleanOption UseColor { get; set; }

    [Option("log-level", Required = false,
      HelpText = "Set log level. Possible values: Trace, Debug, Information, Warning, Error, Critical, None.",
      Default = LogLevel.Error)]
    public LogLevel LogLevel { get; set; }

    [Option("repository", Required = false, HelpText = "Path to the root of a project.")]
    public string Repository { get; set; }

    [Option("token", Required = false, HelpText = "Ghacu.GitHub token to work with actions repositories.")]
    public string GitHubToken { get; set; }

    [Option("upgrade", Required = false, HelpText = "Upgrade versions to the latest one.")]
    public bool Upgrade { get; set; }
  }
}