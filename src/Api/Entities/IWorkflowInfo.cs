using Api.Entities;
using GHACU.Workflow.Entities;

namespace GHACU.Workflow
{
  public interface IWorkflowInfo
  {
    IWorkflowFile File { get; }
    IActionWorkflow Workflow { get; }
  }
}