using System;

namespace GHACU.Workflow.Exceptions
{
  public abstract class WorkflowException : Exception
  {
    public WorkflowException(string? message) : base(message)
    {
    }
  }
}