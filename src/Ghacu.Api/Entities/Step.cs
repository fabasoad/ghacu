using System;
using YamlDotNet.Serialization;

namespace Ghacu.Api.Entities
{
  public sealed class Step
  {
    private Uses _uses;

    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }
    public Uses Uses => _uses ??= new Uses(UsesFullName);
    public bool IsInternal => UsesFullName == null || "./".Equals(UsesFullName);
    public bool IsUpToDate => IsInternal || Uses.CurrentVersion.CompareTo(Uses.GetLatestVersion()) >= 0;
  }
}