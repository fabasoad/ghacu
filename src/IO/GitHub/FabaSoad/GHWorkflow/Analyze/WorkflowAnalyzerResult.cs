using System.Collections.Generic;

namespace IO.GitHub.FabaSoad.GHWorkflow.Analyze
{
  public class WorkflowAnalyzerResult
  {
    public WorkflowAnalyzerResult(string file, string name)
    {
      var index = file.IndexOf(".github");
      File = file.Substring(index);
      Name = name;
    }
    public string File { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<WorkflowAnalyzerAction> Actions { get; internal set; }
  }
}