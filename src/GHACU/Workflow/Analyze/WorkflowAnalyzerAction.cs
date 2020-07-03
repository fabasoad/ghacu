using System;
using GHACU.Workflow.Entities;
using Semver;

namespace GHACU.Workflow.Analyze
{
  public sealed class WorkflowAnalyzerAction
  {
    private const string NA = "N/A";

    public WorkflowAnalyzerAction(string name, string originalName, UsesType type)
    {
      Name = name;
      OriginalName = originalName;
      Type = type;
    }

    public string Name { get; }

    internal string OriginalName { get; }

    public string CurrentVersion { get; set; }

    public string LatestVersion { get; set; }

    public bool IsUpToDate
    {
      get
      {
        if (NA.Equals(LatestVersion))
        {
          return true;
        }

        try
        {
          SemVersion cv = ToSemVersion(CurrentVersion);
          SemVersion lv = ToSemVersion(LatestVersion);
          return cv.CompareByPrecedence(lv) >= 0;
        }
        catch (Exception)
        {
          return CurrentVersion.Equals(LatestVersion);
        }
      }
    }

    internal UsesType Type { get; }

    private SemVersion ToSemVersion(string version) =>
      SemVersion.Parse(version.StartsWith('v') ? version.Substring(1) : version);
  }
}