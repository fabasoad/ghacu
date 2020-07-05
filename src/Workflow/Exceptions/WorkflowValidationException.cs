namespace GHACU.Workflow.Exceptions
{
  public class WorkflowValidationException : WorkflowException
  {
    public WorkflowValidationException(string? message) : base(message)
    {
    }
  }
}