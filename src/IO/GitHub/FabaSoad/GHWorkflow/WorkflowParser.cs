using System.IO;
using YamlDotNet.Serialization;
using IO.GitHub.FabaSoad.GHWorkflow.Entities;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;

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

    public IEnumerable<WorkflowInfo> Parse(IEnumerable<string> files)
    {
      foreach (string file in files)
      {
        Workflow wf = null;
        using (var reader = new StreamReader(file))
        {
          wf = _deserializer.Deserialize<Workflow>(reader);
        }
        yield return new WorkflowInfo(file, wf);
      }
    }
  }
}