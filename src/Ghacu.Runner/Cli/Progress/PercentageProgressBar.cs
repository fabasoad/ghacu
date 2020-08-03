using System;
using System.Text;
using System.Threading;
using Ghacu.Api.Stream;
using Microsoft.Extensions.Logging;

namespace Ghacu.Runner.Cli.Progress
{
  /// <summary>
  /// An ASCII progress bar.
  /// </summary>
  public class PercentageProgressBar : IProgressBar
  {
    private const int BLOCK_COUNT = 10;
    private const string ANIMATION = @"|/-\";
    
    private readonly int _totalTicks;
    private readonly IConsoleStreamer _streamer;
    private readonly TimeSpan _animationInterval = TimeSpan.FromSeconds(1.0 / 8);
    private readonly Timer _timer;
    
    private int _currentTick;
    private double _currentProgress;
    private string _currentText = string.Empty;
    private bool _disposed;
    private int _animationIndex;

    public PercentageProgressBar(int totalTicks, IConsoleStreamer streamer)
    {
      _totalTicks = totalTicks;
      _streamer = streamer;
      _timer = new Timer(TimerHandler);

      // A progress bar is only for temporary display in a console window.
      // If the console output is redirected to a file, draw nothing.
      // Otherwise, we'll end up with a lot of garbage in the target file.
      if (!Console.IsOutputRedirected)
      {
        ResetTimer();
      }
    }

    public void Report(double value)
    {
      // Make sure value is in [0..1] range
      value = Math.Max(0, Math.Min(1, value));
      _currentTick++;
      Interlocked.Exchange(ref _currentProgress, value);
    }

    private void TimerHandler(object state)
    {
      lock (_timer)
      {
        if (_disposed)
        {
          return;
        }

        var progressBlockCount = (int)(_currentProgress * BLOCK_COUNT);
        var percent = (int)(_currentProgress * 100);
        string text = string.Format("[{0}{1}] [{2}/{3}] {4,5}% {5}",
          new string('#', progressBlockCount),
          new string('-', BLOCK_COUNT - progressBlockCount),
          _currentTick,
          _totalTicks,
          percent,
          ANIMATION[_animationIndex++ % ANIMATION.Length]);
        UpdateText(text);
        ResetTimer();
      }
    }

    private void UpdateText(string text)
    {
      // Get length of common portion
      int commonPrefixLength = 0;
      int commonLength = Math.Min(_currentText.Length, text.Length);
      while (commonPrefixLength < commonLength && text[commonPrefixLength] == _currentText[commonPrefixLength])
      {
        commonPrefixLength++;
      }

      // Backtrack to the first differing character
      var outputBuilder = new StringBuilder();
      outputBuilder.Append('\b', _currentText.Length - commonPrefixLength);

      // Output new suffix
      outputBuilder.Append(text.Substring(commonPrefixLength));

      // If the new text is shorter than the old one: delete overlapping characters
      int overlapCount = _currentText.Length - text.Length;
      if (overlapCount > 0)
      {
        outputBuilder.Append(' ', overlapCount);
        outputBuilder.Append('\b', overlapCount);
      }

      _streamer.Push<PercentageProgressBar>(new StreamOptions
      {
        Level = LogLevel.Information,
        Messages = new StreamMessageBuilder().Add(outputBuilder.ToString()).Build()
      });
      _currentText = text;
    }

    private void ResetTimer() => _timer.Change(_animationInterval, TimeSpan.FromMilliseconds(-1));

    public void Dispose()
    {
      lock (_timer)
      {
        _disposed = true;
        UpdateText(string.Empty);
      }
    }
  }
}