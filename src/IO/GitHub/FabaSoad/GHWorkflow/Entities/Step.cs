using YamlDotNet.Serialization;

namespace IO.GitHub.FabaSoad.GHWorkflow.Entities
{
  public class Step
  {
    [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string UsesFullName { get; set; }
    public Uses Uses
    {
      get { return new Uses(this.UsesFullName); }
    }
  }
}