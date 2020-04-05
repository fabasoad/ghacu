using System.Collections.Generic;
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
      var content = System.IO.File.ReadAllText(_originalFilePath);
      foreach (var a in Actions)
      {
        var delimeter = a.Type == UsesType.DOCKER ? ":" : "@";
        var prefix = a.Type == UsesType.DOCKER ? "docker://" : "";
        content = content.Replace(a.OriginalName, $"{prefix}{a.Name}{delimeter}{a.LatestVersion}");
      }
      System.IO.File.WriteAllText(_originalFilePath, content);
    }
  }
}