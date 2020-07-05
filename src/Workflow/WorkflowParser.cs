using System.Collections.Generic;
using System.IO;
using GHACU.Workflow.Entities;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GHACU.Workflow
{
  public sealed class WorkflowParser : IWorkflowParser
  {
    private IDeserializer _deserializer;

    public WorkflowParser()
    {
      _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();
    }

    public IEnumerable<IWorkflowInfo> Parse(IEnumerable<string> files)
    {
      foreach (string file in files)
      {
        ActionWorkflow wf;
        using (var reader = new StreamReader(file))
        {
          wf = _deserializer.Deserialize<ActionWorkflow>(reader);
        }

        yield return new WorkflowInfo(file, wf);
      }
    }
  }
}