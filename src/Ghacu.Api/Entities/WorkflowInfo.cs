using System.Linq;
using System.Text.RegularExpressions;

namespace Ghacu.Api.Entities
{
  public sealed class WorkflowInfo
  {
    public WorkflowInfo(string file, ActionWorkflow wf)
    {
      File = new WorkflowFile(file);
      Workflow = wf;
    }

    public WorkflowFile File { get; }
    public ActionWorkflow Workflow { get; }

    public void Upgrade()
    {
      string content = System.IO.File.ReadAllText(File.FilePath);
      foreach (Step step in Workflow.Jobs.Values
        .SelectMany(job => job.Steps)
        .Where(step => !step.IsUpToDate))
      {
        string delimiter = step.Uses.Type == UsesType.Docker ? ":" : "@";
        string prefix = step.Uses.Type == UsesType.Docker ? "docker://" : string.Empty;
        content = Regex.Replace(
          content,
          $"(.*)({step.UsesFullName}[ \t]*)(\n.*)",
          $"$1{prefix}{step.Uses.FullName}{delimiter}{step.Uses.GetLatestVersion().Value}$3");
      }

      System.IO.File.WriteAllText(File.FilePath, content);
    }
  }
}