using GHACU.Workflow.Entities;

namespace GHACU.Workflow
{
  public sealed class WorkflowInfo
  {
    public WorkflowInfo(string file, ActionWorkflow wf)
    {
      File = file;
      Workflow = wf;
    }

    public string File { get; }
    public ActionWorkflow Workflow { get; }
  }
}