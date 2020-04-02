using IO.GitHub.FabaSoad.GHWorkflow.Entities;

namespace IO.GitHub.FabaSoad.GHWorkflow
{
  public class WorkflowInfo
  {
    public WorkflowInfo(string file, Workflow wf)
    {
      this.File = file;
      this.Workflow = wf;
    }

    public string File { get; private set; }
    public Workflow Workflow { get; private set; }
  }
}