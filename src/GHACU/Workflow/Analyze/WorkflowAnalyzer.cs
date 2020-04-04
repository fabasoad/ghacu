using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GHACU.GitHub;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzer
  {
    private RepositoryScanner _scanner;
    public WorkflowAnalyzer()
    {
      _scanner = new RepositoryScanner();
    }
    
    public async Task<IEnumerable<WorkflowAnalyzerResult>> Analyze(IEnumerable<WorkflowInfo> items)
    {
      List<WorkflowAnalyzerResult> result = new List<WorkflowAnalyzerResult>();
      foreach (var wfi in items)
      {
        var item = new WorkflowAnalyzerResult(wfi.File, wfi.Workflow.Name);
        List<WorkflowAnalyzerAction> actions = new List<WorkflowAnalyzerAction>();
        foreach (var step in wfi.Workflow.Jobs.Values.SelectMany(j => j.Steps.Where(s => s.Uses != null)))
        {
          var action = new WorkflowAnalyzerAction(step.Uses.FullName, step.UsesFullName, step.Uses.Type);
          action.CurrentVersion = step.Uses.Version;
          action.LatestVersion = await _scanner.GetLatestRelease(step.Uses);
          actions.Add(action);
        }
        item.Actions = actions;
        result.Add(item);
      }
      return result;
    }
  }
}