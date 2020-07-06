using Ghacu.Runner.Cli;
using Xunit;

namespace Ghacu.Runner.Tests.Cli
{
  public class OptionValidationExceptionTest
  {
    [Fact]
    public void Create_ReturnsValidMessage()
    {
      var message = "Test Message";
      Assert.Equal(message, new OptionValidationException(message).Message);
    }
  }
}
