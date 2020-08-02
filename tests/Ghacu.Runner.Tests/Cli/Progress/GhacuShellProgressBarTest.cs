using System;
using Ghacu.Api.Stream;
using Ghacu.Runner.Cli.Progress;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.Runner.Tests.Cli.Progress
{
  public class GhacuShellProgressBarTest
  {
    [Fact]
    public void Create_Successfully()
    {
      const int totalTicks = 3;
      var progressBar = new GhacuShellProgressBar(totalTicks, null);
      Assert.Equal($"[0/{totalTicks}]", progressBar.ProgressBar.Message);
      Assert.Equal(totalTicks, progressBar.ProgressBar.MaxTicks);
    }

    [Fact]
    public void Tick_Positive()
    {
      const int currentTick = 12;
      const int maxTicks = 34;
      var shellProgressBarMock = Mock.Create<ShellProgressBar.IProgressBar>();
      Mock.Arrange(() => shellProgressBarMock.CurrentTick).Returns(currentTick);
      Mock.Arrange(() => shellProgressBarMock.MaxTicks).Returns(maxTicks);
      Mock.Arrange(() => shellProgressBarMock.Tick($"[{currentTick + 1}/{maxTicks}]"))
        .DoNothing().OccursOnce();

      var progressBar = new GhacuShellProgressBar(null, options =>
      {
        Assert.True(options.CollapseWhenFinished);
        Assert.True(options.DisplayTimeInRealTime);
        Assert.Equal('#', options.ProgressCharacter);
        Assert.True(options.ProgressBarOnBottom);
        Assert.True(options.ShowEstimatedDuration);
        Assert.Equal(Console.ForegroundColor, options.ForegroundColor);
        Assert.Equal('-', options.BackgroundCharacter);
        return shellProgressBarMock;
      });
      Assert.Equal(shellProgressBarMock, progressBar.ProgressBar);
      progressBar.Report(-2.1 /* any double */);
      
      Mock.Assert(shellProgressBarMock);
    }

    [Fact]
    public void Dispose_Positive()
    {
      var shellProgressBarMock = Mock.Create<ShellProgressBar.IProgressBar>();
      Mock.Arrange(() => shellProgressBarMock.Dispose()).DoNothing().OccursOnce();
      var streamerMock = Mock.Create<IStreamer>();
      Mock.Arrange(() => streamerMock.Clear(2)).DoNothing().OccursOnce();
      var progressBar = new GhacuShellProgressBar(streamerMock, _ => shellProgressBarMock);
      progressBar.Dispose();
      Mock.Assert(shellProgressBarMock);
      Mock.Assert(streamerMock);
    }
  }
}