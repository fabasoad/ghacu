using Ghacu.Runner.Cli.Stream;
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
    
    public override void PrintHeader(string workflowName, string fileName) =>
      _streamer.PushLine("> {0} ({1})", workflowName, fileName);

    protected override void Print(string template, GitHubAction action) => _streamer.PushLine(
      template, action.Repository, action.CurrentVersion, ArrowChar, action.LatestVersion);

    public override void PrintNoUpgradeNeeded() => _streamer.PushLine("All GitHub Actions match the latest versions :)");

    public override void PrintRunUpgrade() => _streamer.PushLine("Run ghacu --upgrade to upgrade the actions.");
  }
}