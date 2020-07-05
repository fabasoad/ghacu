using System;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace GHACU.Workflow.Entities
{
  public sealed class Step : IStep
  {
    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }
    public IUses Uses => UsesFullName == null || "./".Equals(UsesFullName) ? null : new Uses(UsesFullName);
    public bool IsUpToDate
    {
      get
      {
        try
        {
          return Uses.CurrentVersion.CompareByPrecedence(Uses.LatestVersion) >= 0;
        }
        catch (Exception)
        {
          return Uses.CurrentVersion.Equals(Uses.LatestVersion);
        }
      }
    }

    public void Upgrade(string fileName)
    {
      string content = System.IO.File.ReadAllText(fileName);
      string delimiter = Uses.Type == UsesType.DOCKER ? ":" : "@";
      string prefix = Uses.Type == UsesType.DOCKER ? "docker://" : string.Empty;
      content = Regex.Replace(
        content,
        $"(.*)({UsesFullName}[ \t]*)(\n.*)",
        $"$1{prefix}{Uses.FullName}{delimiter}{Uses.LatestVersion}$3");
      System.IO.File.WriteAllText(fileName, content);
    }
  }
}