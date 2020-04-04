using YamlDotNet.Serialization;

namespace GHACU.Workflow.Entities
{
  public sealed class Step
  {
    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }
    public Uses Uses
    {
      get { return UsesFullName == null ? null : new Uses(this.UsesFullName); }
    }
  }
}