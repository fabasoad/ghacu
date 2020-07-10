using System;

namespace Ghacu.GitHub.Exceptions
{
  public abstract class GitHubException : Exception
  {
    protected GitHubException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }
}