using GHACU.Workflow.Entities;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzerAction
  {
    public WorkflowAnalyzerAction(string name, string originalName, UsesType type)
    {
      Name = name;
      OriginalName = originalName;
      Type = type;
    }
    public string Name { get; private set; }
    internal string OriginalName { get; private set; }
    public string CurrentVersion { get; set; }
    public string LatestVersion { get; set; }
    public bool IsUpToDate { get => CurrentVersion.Equals(LatestVersion); }
    internal UsesType Type { get; private set; }
  }
}