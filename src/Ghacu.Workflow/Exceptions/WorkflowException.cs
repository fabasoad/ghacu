using System;

namespace Ghacu.Workflow.Exceptions
{
  public abstract class WorkflowException : Exception
  {
    protected WorkflowException(string message) : base(message)
    {
    }
  }
}