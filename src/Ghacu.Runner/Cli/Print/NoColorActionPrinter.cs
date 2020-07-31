using Ghacu.Api.Stream;
using Ghacu.Runner.Cli.Stream;
using Microsoft.Extensions.Logging;
using GitHubAction = Ghacu.Api.Entities.Action;

namespace Ghacu.Runner.Cli.Print
{
  public class NoColorActionPrinter : ActionPrinterBase
  {
    private readonly IStreamer _streamer;

    public NoColorActionPrinter(IStreamer streamer)
    {
      _streamer = streamer;
    }
    
    public override void PrintHeader(string workflowName, string fileName) => _streamer.PushLine<NoColorActionPrinter>(
      new StreamOptions { Level = LogLevel.Information, Message = $"> {workflowName} ({fileName})" });

    protected override void Print(string template, GitHubAction action) =>
      _streamer.PushLine<NoColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Message = string.Format(template, action.Repository, action.CurrentVersion, ArrowChar, action.LatestVersion)
      });

    public override void PrintNoUpgradeNeeded() => _streamer.PushLine<NoColorActionPrinter>(
      new StreamOptions { Level = LogLevel.Information, Message = "All GitHub Actions match the latest versions :)" });

    public override void PrintRunUpgrade() => _streamer.PushLine<NoColorActionPrinter>(
      new StreamOptions { Level = LogLevel.Information, Message = "Run ghacu --upgrade to upgrade the actions." });
  }
}