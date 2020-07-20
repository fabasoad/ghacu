using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class StepTest
  {
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("./")]
    [InlineData("random-string")]
    [InlineData("valid/string")]
    [InlineData("valid/string@version")]
    [InlineData("docker://invalid/string@version")]
    [InlineData("docker://valid/string:version")]
    [InlineData("docker://invalid/string:version:version")]
    [InlineData("docker://docker://invalid/string:version")]
    [InlineData("docker://docker://invalid/string:version:version")]
    [InlineData("docker://invalid/invalid/string@version")]
    [InlineData("docker://invalid/invalid/string@version@version")]
    [InlineData("docker://docker://invalid/invalid/string@version@version")]
    [InlineData("valid/valid/valid/string")]
    [InlineData("valid/valid/valid/string@version")]
    [InlineData("valid/valid/valid/string@version@invalid")]
    public void Test_GettersSetters(string uses)
    {
      var step = new Step { Uses = uses };
      Assert.Equal(uses, step.Uses);
      Action action = step.Action;
      Assert.NotNull(action);
      Assert.Same(action, step.Action);
    }
  }
}