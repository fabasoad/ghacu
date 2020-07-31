using System;

namespace Ghacu.GitHub
{
  public class RepositoryCheckedArgs
  {
    private readonly int _index;
    private readonly int _totalCount;

    public RepositoryCheckedArgs(int index, int totalCount)
    {
      _index = index;
      _totalCount = totalCount;
    }

    public double ProgressValue => Math.Max(0, Math.Min(1, (double)_index / _totalCount));
  }
}