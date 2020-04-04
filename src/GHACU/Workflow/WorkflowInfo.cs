using GHACU.Workflow.Entities;

namespace GHACU.Workflow
{
  public sealed class WorkflowInfo
  {
    public WorkflowInfo(string file, ActionWorkflow wf)
    {
      this.File = file;
      this.Workflow = wf;
    }

    public string File { get; private set; }
    public ActionWorkflow Workflow { get; private set; }
  }
}