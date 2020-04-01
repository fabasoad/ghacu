using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHWorkflow
{
  public class WorkflowInfo
  {
    public WorkflowInfo(string file)
    {
      this.File = file;
    }

    public string File { get; private set; }
    public IEnumerable<Action> Actions { get; }
  }
}