using System;
using Ghacu.Api.Stream;
using ShellProgressBar;

namespace Ghacu.Runner.Cli.Progress
{
  public class GhacuShellProgressBar : IProgressBar
  {
    private readonly IStreamer _streamer;

    public GhacuShellProgressBar(int totalTicks, IStreamer streamer)
      : this(streamer, o => new ProgressBar(totalTicks, $"[0/{totalTicks}]", o))
    {
    }

    internal GhacuShellProgressBar(IStreamer streamer, Func<ProgressBarOptions, ShellProgressBar.IProgressBar> factory)
    {
      _streamer = streamer;
      ProgressBar = factory(new ProgressBarOptions
      {
        CollapseWhenFinished = true,
        DisplayTimeInRealTime = true,
        ProgressCharacter = '#',
        ProgressBarOnBottom = true,
        ShowEstimatedDuration = true,
        ForegroundColor = Console.ForegroundColor,
        BackgroundCharacter = '-'
      });
    }
    
    internal ShellProgressBar.IProgressBar ProgressBar { get; }

    public void Dispose()
    {
      ProgressBar.Dispose();
      _streamer.Clear(2);
    }

    public void Report(double value) =>
      ProgressBar.Tick($"[{ProgressBar.CurrentTick + 1}/{ProgressBar.MaxTicks}]");
  }
}