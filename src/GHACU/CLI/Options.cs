using CommandLine;
using Microsoft.Extensions.Logging;

namespace GHACU.CLI
{
  public sealed class Options
  {
    [Option('l', "log-level", Required = false, HelpText = "Set log level. Possible values: Trace, Debug, Information, Warning, Error, Critical, None.", Default = LogLevel.Error)]
    public LogLevel LogLevel { get; set; }

    [Option('n', "no-cache", Required = false, HelpText = "Turn it on if you do not want to use caching.")]
    public bool NoCache { get; set; }

    [Option('r', "repository", Required = false, HelpText = "Path to the root of a project.")]
    public string Repository { get; set; }

    [Option('t', "token", Required = false, HelpText = "GitHub token to work with actions repositories.")]
    public string GitHubToken { get; set; }

    [Option('u', "upgrade", Required = false, HelpText = "Upgrade versions to the latest one.")]
    public bool Upgrade { get; set; }
  }
}