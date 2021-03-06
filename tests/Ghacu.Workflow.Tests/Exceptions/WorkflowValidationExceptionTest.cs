using Ghacu.Workflow.Exceptions;
using Xunit;

namespace Ghacu.Workflow.Tests.Exceptions
{
  public class WorkflowValidationExceptionTest
  {
    [Fact]
    public void Create_ReturnsValidMessage()
    {
      const string message = "Test Message";
      Assert.Equal(message, new WorkflowValidationException(message).Message);
    }
  }
}