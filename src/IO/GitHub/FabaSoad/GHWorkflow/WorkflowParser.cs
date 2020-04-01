using System.IO;
using YamlDotNet.Serialization;
using IO.GitHub.FabaSoad.GHWorkflow.Entities;
using YamlDotNet.Serialization.NamingConventions;

namespace IO.GitHub.FabaSoad.GHWorkflow
{
  public class WorkflowParser
  {
    private IDeserializer _deserializer;
    public WorkflowParser()
    {
      _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    }

    public Workflow Parse(string path)
    {
      Workflow wf = null;
      using (var reader = new StreamReader(path))
      {
        wf = _deserializer.Deserialize<Workflow>(reader);
      }
      return wf;
    }
  }
}