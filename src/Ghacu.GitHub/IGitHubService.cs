using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.GitHub
{
  public interface IGitHubService
  {
    IDictionary<WorkflowInfo, IEnumerable<Action>> GetOutdated(IEnumerable<WorkflowInfo> items);
  }
}