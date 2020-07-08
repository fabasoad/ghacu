using System.Collections.Generic;

namespace Ghacu.Api.Entities
{
  public sealed class Job
  {
    public IEnumerable<Step> Steps { get; set; }
  }
}