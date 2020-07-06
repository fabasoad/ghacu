using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.GitHub
{
  public interface IGitHubService
  {
    IDictionary<WorkflowInfo, IEnumerable<Step>> GetOutdated(IEnumerable<WorkflowInfo> items);
  }
}