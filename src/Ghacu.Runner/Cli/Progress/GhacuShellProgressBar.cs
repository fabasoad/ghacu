using System;
using ShellProgressBar;

namespace Ghacu.Runner.Cli.Progress
{
  public class GhacuShellProgressBar : IProgressBar
  {
    private readonly ProgressBar _progressBar;

    public GhacuShellProgressBar(int totalTicks)
    {
      _progressBar = new ProgressBar(totalTicks, $"[0/{totalTicks}]", new ProgressBarOptions
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

    public void Dispose()
    {
      _progressBar.Dispose();
      ClearCurrentConsoleLine();
    }

    private static void ClearCurrentConsoleLine()
    {
      for (var i = 0; i < 2; i++)
      {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, currentLineCursor);
      }
    }

    public void Report(double value) =>
      _progressBar.Tick($"[{_progressBar.CurrentTick + 1}/{_progressBar.MaxTicks}]");
  }
}