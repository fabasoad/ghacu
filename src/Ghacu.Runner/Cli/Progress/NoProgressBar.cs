namespace Ghacu.Runner.Cli.Progress
{
  public class NoProgressBar : IProgressBar
  {
    public void Dispose()
    {
    }

    public void Report(double value)
    {
    }
  }
}