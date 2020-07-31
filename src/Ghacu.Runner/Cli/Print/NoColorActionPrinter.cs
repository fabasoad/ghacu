using Ghacu.Api.Stream;
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
      new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder().Add($"> {workflowName} ({fileName})").Build()
      });

    protected override void Print(string template, GitHubAction action) =>
      _streamer.PushLine<NoColorActionPrinter>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder()
          .Add(string.Format(template, action.Repository, action.CurrentVersion, ArrowChar, action.LatestVersion))
          .Build()
      });

    public override void PrintNoUpgradeNeeded() => _streamer.PushLine<NoColorActionPrinter>(new StreamOptions
    {
      Level = LogLevel.Information,
      Messages = new StreamMessageBuilder().Add("All GitHub Actions match the latest versions :)").Build()
    });

    public override void PrintRunUpgrade() => _streamer.PushLine<NoColorActionPrinter>(new StreamOptions
    {
      Level = LogLevel.Information,
      Messages = new StreamMessageBuilder().Add("Run ghacu --upgrade to upgrade the actions.").Build()
    });
  }
}