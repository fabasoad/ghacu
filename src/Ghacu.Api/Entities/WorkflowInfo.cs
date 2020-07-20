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
      foreach (Action action in Workflow.Jobs.Values
        .SelectMany(job => job.Steps)
        .Select(step => step.Action)
        .Where(action => !action.IsUpToDate)
        .Distinct())
      {
        string delimiter = action.Type == UsesType.Docker ? ":" : "@";
        string prefix = action.Type == UsesType.Docker ? "docker://" : string.Empty;
        content = Regex.Replace(
          content,
          $"(.*)({action.FullName}{delimiter}.+[ \t]*)(\n.*)",
          $"$1{prefix}{action.FullName}{delimiter}{action.LatestVersion}$3");
      }

      System.IO.File.WriteAllText(File.FilePath, content);
    }
  }
}