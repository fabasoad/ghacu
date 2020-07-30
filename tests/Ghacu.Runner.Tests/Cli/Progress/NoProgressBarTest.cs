using Ghacu.Runner.Cli.Progress;
using Xunit;

namespace Ghacu.Runner.Tests.Cli.Progress
{
  public class NoProgressBarTest
  {
    [Fact]
    public void Dispose_Passed() => new NoProgressBar().Dispose();

    [Theory]
    [InlineData(0.0)]
    [InlineData(-4.34)]
    [InlineData(14.04)]
    public void Report_Passed(double v) => new NoProgressBar().Report(v);
  }
}