namespace Ghacu.Runner.Cli
{
  public class ProgressBarValue
  {
    public int Index { get; }
    public int TotalCount { get; }
    
    public double ProgressValue => (double)Index / TotalCount;

    public ProgressBarValue(int index, int totalCount)
    {
      Index = index;
      TotalCount = totalCount;
    }
  }
}