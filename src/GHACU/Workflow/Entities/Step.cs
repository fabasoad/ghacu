using YamlDotNet.Serialization;

namespace GHACU.Workflow.Entities
{
  public sealed class Step
  {
    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }
    public Uses Uses
    {
      get { return (UsesFullName == null || "./".Equals(UsesFullName)) ? null : new Uses(this.UsesFullName); }
    }
  }
}