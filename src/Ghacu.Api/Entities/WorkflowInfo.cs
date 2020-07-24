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
      if (string.IsNullOrWhiteSpace(File.FilePath) || !System.IO.File.Exists(File.FilePath))
      {
        return;
      }

      string content = System.IO.File.ReadAllText(File.FilePath);
      foreach (Action action in Workflow.Jobs.Values
        .SelectMany(job => job.Steps)
        .Select(step => step.Action)
        .Where(action => !action.IsUpToDate)
        .Distinct())
      {
        string delimiter = action.Type == UsesType.Docker ? ":" : "@";
        content = Regex.Replace(
          content,
          $"(.*)({action.FullName}{delimiter}.+[ \t]*)(.*)",
          $"$1{action.FullName}{delimiter}{action.LatestVersion}$3");
      }

      System.IO.File.WriteAllText(File.FilePath, content);
    }
  }
}