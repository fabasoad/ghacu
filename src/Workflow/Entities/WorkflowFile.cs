using Api.Entities;

namespace Workflow.Entities
{
  public class WorkflowFile : IWorkflowFile
  {
    public WorkflowFile(string path)
    {
      FilePath = path;
    }

    public string Name => FilePath.Substring(FilePath.IndexOf(".github"));
    public string FilePath { get; }
  }
}