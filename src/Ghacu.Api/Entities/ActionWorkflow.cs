using System.Collections.Generic;

namespace Ghacu.Api.Entities
{
  public sealed class ActionWorkflow
  {
    public string Name { get; set; }
    public Dictionary<string, Job> Jobs { get; set; }
  }
}