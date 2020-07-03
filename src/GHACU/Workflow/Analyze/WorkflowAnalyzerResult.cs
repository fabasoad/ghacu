using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GHACU.Workflow.Entities;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzerResult
  {
    private string _originalFilePath;
    public WorkflowAnalyzerResult(string file, string name, IEnumerable<WorkflowAnalyzerAction> actions)
    {
      _originalFilePath = file;
      var index = file.IndexOf(".github");
      File = file.Substring(index);
      Name = name;
      Actions = actions;
    }

    public string File { get; private set; }

    public string Name { get; private set; }

    public IEnumerable<WorkflowAnalyzerAction> Actions { get; private set; }

    public void Upgrade()
    {
      var actionsForUpdate = Actions.Where(a => !a.IsUpToDate);
      if (actionsForUpdate.Count() > 0)
      {
        var content = System.IO.File.ReadAllText(_originalFilePath);
        foreach (var a in actionsForUpdate)
        {
          var delimeter = a.Type == UsesType.DOCKER ? ":" : "@";
          var prefix = a.Type == UsesType.DOCKER ? "docker://" : string.Empty;
          content = Regex.Replace(content, $"{a.OriginalName}\\s*$", $"{prefix}{a.Name}{delimeter}{a.LatestVersion}");
        }

        System.IO.File.WriteAllText(_originalFilePath, content);
      }
    }
  }
}