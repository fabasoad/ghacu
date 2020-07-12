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
      foreach (Uses uses in Workflow.Jobs.Values
        .SelectMany(job => job.Steps)
        .Where(step => !step.IsUpToDate)
        .Select(step => step.Uses)
        .Distinct())
      {
        string delimiter = uses.Type == UsesType.Docker ? ":" : "@";
        string prefix = uses.Type == UsesType.Docker ? "docker://" : string.Empty;
        content = Regex.Replace(
          content,
          $"(.*)({uses.FullName}{delimiter}.+[ \t]*)(\n.*)",
          $"$1{prefix}{uses.FullName}{delimiter}{uses.GetLatestVersion().Value}$3");
      }

      System.IO.File.WriteAllText(File.FilePath, content);
    }
  }
}