using CommandLine;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli
{
  public sealed class Options
  {
    [Option("no-cache", Required = false, HelpText = "Disable cache.", Default = false)]
    public bool NoCache { get; set; }

    [Option("no-colors", Required = false, HelpText = "Disable colors.", Default = false)]
    public bool NoColors { get; set; }

    [Option("log-level", Required = false,
      HelpText = "Set log level. Possible values: Trace, Debug, Information, Warning, Error, Critical, None.",
      Default = LogLevel.Information)]
    public LogLevel LogLevel { get; set; }

    [Option("output-type", Required = false,
      HelpText = "Information output type. Possible values: Console, Logger, Silent.",
      Default = OutputType.Console)]
    public OutputType OutputType { get; set; }

    [Option("repository", Required = false, HelpText = "Path to the root of a project.")]
    public string Repository { get; set; }

    [Option("token", Required = false, HelpText = "GitHub token to work with actions repositories.")]
    public string GitHubToken { get; set; }

    [Option("upgrade", Required = false, HelpText = "Upgrade versions to the latest one.")]
    public bool Upgrade { get; set; }
  }
}