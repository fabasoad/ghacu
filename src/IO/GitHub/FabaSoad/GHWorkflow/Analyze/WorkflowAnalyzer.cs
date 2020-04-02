using System.Collections.Generic;
using System.Linq;
using IO.GitHub.FabaSoad.GHApi;

namespace IO.GitHub.FabaSoad.GHWorkflow.Analyze
{
  public class WorkflowAnalyzer
  {
    private RepositoryScanner _scanner;
    public WorkflowAnalyzer()
    {
      _scanner = new RepositoryScanner();
    }
    
    public IEnumerable<WorkflowAnalyzerResult> Analyze(IEnumerable<WorkflowInfo> items)
    {
      foreach (var wfi in items)
      {
        var item = new WorkflowAnalyzerResult(wfi.File, wfi.Workflow.Name);
        List<WorkflowAnalyzerAction> actions = new List<WorkflowAnalyzerAction>();
        foreach (var step in wfi.Workflow.Jobs.Values.SelectMany(j => j.Steps))
        {
          var info = _scanner.Scan(step.Uses.Repository);
          var action = new WorkflowAnalyzerAction(step.Uses.Repository);
          action.CurrentVersion = step.Uses.Version;
          action.LatestVersion = info.Tags.LastOrDefault() ?? "unknown"; // TODO: get latest version
          actions.Add(action);
        }
        item.Actions = actions;
        yield return item;
      }
    }
  }
}