namespace Ghacu.GitHub
{
  public class RepositoryCheckedArgs
  {
    public int Index { get; }
    public int TotalCount { get; }

    public RepositoryCheckedArgs(int index, int totalCount)
    {
      Index = index;
      TotalCount = totalCount;
    }
  }
}