using System.Collections.Generic;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzerAction
  {
    public WorkflowAnalyzerAction(string name)
    {
      Name = name;
    }
    public string Name { get; private set; }
    public string CurrentVersion { get; set; }
    public string LatestVersion { get; set; }
    public bool IsUpToDate { get => CurrentVersion.Equals(LatestVersion); }
  }
}