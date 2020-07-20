using Ghacu.GitHub.Exceptions;
using Xunit;

namespace Ghacu.GitHub.Tests.Exceptions
{
  public class GitHubVersionNotFoundExceptionTest
  {
    [Fact]
    public void Create_Successfully()
    {
      const string message1 = "test-message-1";
      var e1 = new GitHubVersionNotFoundException(message1);
      Assert.Equal(message1, e1.Message);
      Assert.Null(e1.InnerException);
      
      const string message2 = "test-message-2";
      var e2 = new GitHubVersionNotFoundException(message2, e1);
      Assert.Equal(message2, e2.Message);
      Assert.Equal(e1, e2.InnerException);
    }
  }
}