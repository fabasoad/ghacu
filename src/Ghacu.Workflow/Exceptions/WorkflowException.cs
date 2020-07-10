using System;

namespace Ghacu.Workflow.Exceptions
{
  public abstract class WorkflowException : Exception
  {
    public WorkflowException(string message) : base(message)
    {
    }
  }
}