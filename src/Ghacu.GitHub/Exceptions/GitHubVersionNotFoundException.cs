using System;

namespace Ghacu.GitHub.Exceptions
{
  public class GitHubVersionNotFoundException : GitHubException
  {
    public GitHubVersionNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    public GitHubVersionNotFoundException(string message) : base(message)
    {
    }
  }
}