using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GHACU.GitHub;
using GHACU.Workflow.Entities;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzer
  {
    private RepositoryScanner _scanner;
    public WorkflowAnalyzer(string gitHubToken)
    {
      _scanner = new RepositoryScanner(gitHubToken);
    }

    public async Task<WorkflowAnalyzerResultList> Analyze(IEnumerable<WorkflowInfo> items)
    {
      WorkflowAnalyzerResultList result = new WorkflowAnalyzerResultList();
      foreach (var wfi in items)
      {
        var actions = new List<WorkflowAnalyzerAction>();
        foreach (Step step in wfi.Workflow.Jobs.Values.SelectMany(j => j.Steps.Where(s => s.Uses != null)))
        {
          var action = new WorkflowAnalyzerAction(step.Uses.FullName, step.UsesFullName, step.Uses.Type);
          action.CurrentVersion = step.Uses.Version;
          action.LatestVersion = await _scanner.GetLatestVersion(step.Uses);
          actions.Add(action);
        }

        result.Add(new WorkflowAnalyzerResult(wfi.File, wfi.Workflow.Name, actions));
      }

      return result;
    }
  }
}