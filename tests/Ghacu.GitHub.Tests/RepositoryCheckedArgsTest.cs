using Xunit;

namespace Ghacu.GitHub.Tests
{
  public class RepositoryCheckedArgsTest
  {
    [Theory]
    [InlineData(1, 10, 0.1)]
    [InlineData(8, 8, 1)]
    [InlineData(14, 13, 1)]
    [InlineData(0, 8, 0)]
    [InlineData(7, 0, 1)]
    [InlineData(-97, 60, 0)]
    [InlineData(4, -3, 0)]
    public void ProgressValue_Correct(int index, int totalCount, double expected) =>
      Assert.Equal(expected, new RepositoryCheckedArgs(index, totalCount).ProgressValue);
  }
}