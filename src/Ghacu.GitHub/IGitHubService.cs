using System.Collections.Generic;
using Ghacu.Api.Entities;

namespace Ghacu.GitHub
{
  public interface IGitHubService
  {
    event System.Action<RepositoryCheckedArgs> RepositoryChecked;
    event System.Action RepositoryCheckedFinished;
    event System.Action<int> RepositoryCheckedStarted;
    
    /// <summary>
    /// Gets outdated actions.
    /// </summary>
    /// <param name="items">Collection of <see cref="WorkflowInfo"/> instances to analyze.</param>
    /// <returns>Dictionary, where key is <see cref="WorkflowInfo"/> and value is a list of <see cref="Action"/> instances.</returns>
    IDictionary<WorkflowInfo, IEnumerable<Action>> GetOutdated(IEnumerable<WorkflowInfo> items);
  }
}