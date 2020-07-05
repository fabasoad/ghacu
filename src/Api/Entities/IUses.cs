using Semver;

namespace GHACU.Workflow.Entities
{
  public interface IUses
  {
    string FullName { get; }
    string Owner { get; }
    string ActionName { get; }
    SemVersion CurrentVersion { get; }
    SemVersion LatestVersion { get; }
    UsesType Type { get; }
    void UpdateLatestVersion(string version);
  }
}