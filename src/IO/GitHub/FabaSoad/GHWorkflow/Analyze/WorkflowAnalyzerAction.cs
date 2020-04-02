using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHWorkflow.Analyze
{
  public class WorkflowAnalyzerAction
  {
    public WorkflowAnalyzerAction(string name)
    {
      Name = name;
    }
    public string Name { get; private set; }
    public string CurrentVersion { get; set; }
    public string LatestVersion { get; set; }
  }
}