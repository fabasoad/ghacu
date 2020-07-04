using System.Collections.Generic;
using System.Linq;
using GHACU.GitHub;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzer
  {
    private readonly RepositoryScanner _scanner;

    public WorkflowAnalyzer(string gitHubToken)
    {
      _scanner = new RepositoryScanner(gitHubToken);
    }

    public IEnumerable<WorkflowAnalyzerResult> GetOutdated(IEnumerable<WorkflowInfo> items) => items
      .AsParallel()
      .Select(wfi => new
      {
        Key = wfi,
        Value = wfi.Workflow.Jobs.Values
          .AsParallel()
          .SelectMany(j => j.Steps.Where(s => s.Uses != null))
          .Select(async step => new WorkflowAnalyzerAction(step.Uses.FullName, step.UsesFullName, step.Uses.Type)
          {
            CurrentVersion = step.Uses.Version,
            LatestVersion = await _scanner.GetLatestVersion(step.Uses)
          })
          .Select(t => t.Result)
          .Where(a => !a.IsUpToDate)
          .ToList()
      })
      .Where(p => p.Value.Count > 0)
      .Select(p => new WorkflowAnalyzerResult(p.Key.File, p.Key.Workflow.Name, p.Value));
  }
}