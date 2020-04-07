using System.Collections.Generic;
using System.Linq;

namespace GHACU.Workflow.Analyze
{
  public class WorkflowAnalyzerResultList : List<WorkflowAnalyzerResult>
  {
    public IEnumerable<WorkflowAnalyzerResult> SkipUpToDate() =>
      this.Where(r => r.Actions.Any(a => !a.IsUpToDate));
  }
}