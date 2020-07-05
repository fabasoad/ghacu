using Api.Entities;
using GHACU.Workflow.Entities;
using Workflow.Entities;

namespace GHACU.Workflow
{
  public sealed class WorkflowInfo : IWorkflowInfo
  {
    public WorkflowInfo(string file, IActionWorkflow wf)
    {
      File = new WorkflowFile(file);
      Workflow = wf;
    }

    public IWorkflowFile File { get; }
    public IActionWorkflow Workflow { get; }
  }
}