namespace Ghacu.Runner.Tests.Cli
{
  using Ghacu.Runner.Cli;
  using Xunit;

  public class OptionValidationExceptionTest
  {
    [Fact]
    public void Create_ReturnsValidMessage()
    {
      const string message = "Test Message";
      Assert.Equal(message, new OptionValidationException(message).Message);
    }
  }
}