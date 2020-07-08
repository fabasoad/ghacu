using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ghacu.Api.Entities;

[assembly: InternalsVisibleTo("Ghacu.Workflow.Tests")]
namespace Ghacu.Workflow
{
  public interface IWorkflowService
  {
    IEnumerable<WorkflowInfo> GetWorkflows(string repositoryPath);
  }
}