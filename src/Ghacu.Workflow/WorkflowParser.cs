using System.Collections.Generic;
using System.IO;
using Ghacu.Api.Entities;
using YamlDotNet.Serialization;

namespace Ghacu.Workflow
{
  public sealed class WorkflowParser : IWorkflowParser
  {
    private readonly IDeserializer _deserializer;

    public WorkflowParser(IDeserializerFactory deserializerFactory)
    {
      _deserializer = deserializerFactory.Get();
    }

    public IEnumerable<WorkflowInfo> Parse(IEnumerable<string> files)
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