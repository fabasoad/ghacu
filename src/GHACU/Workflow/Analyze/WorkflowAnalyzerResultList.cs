using System.Linq;
using System.Collections.Generic;

namespace GHACU.Workflow.Analyze
{
  public class WorkflowAnalyzerResultList : List<WorkflowAnalyzerResult>
  {
    public IEnumerable<WorkflowAnalyzerResult> SkipUpToDate() =>
      this.Where(r => r.Actions.Any(a => !a.IsUpToDate));
  }
}