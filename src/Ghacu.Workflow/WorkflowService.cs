using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.Workflow.Exceptions;

namespace Ghacu.Workflow
{
  public class WorkflowService : IWorkflowService
  {
    private const string GITHUB_FOLDER = ".github";
    private const string WORKFLOWS_FOLDER = "workflows";

    private readonly IWorkflowParser _workflowParser;

    public WorkflowService(IWorkflowParser workflowParser)
    {
      _workflowParser = workflowParser;
    }

    public IEnumerable<WorkflowInfo> GetWorkflows(string repositoryPath)
    {
      string wfPath = GetWorkflowPath(repositoryPath);
      return _workflowParser.Parse(new[] { "*.yml", "*.yaml" }
        .SelectMany(p => Directory.EnumerateFiles(wfPath, p, SearchOption.AllDirectories)));
    }

    private string GetWorkflowPath(string repositoryPath)
    {
      string rep;
      if (string.IsNullOrWhiteSpace(repositoryPath))
      {
        rep = Directory.GetCurrentDirectory();
      }
      else
      {
        if (!Directory.Exists(repositoryPath))
        {
          throw new WorkflowValidationException($"Directory {repositoryPath} does not exist.");
        }

        rep = repositoryPath;
      }

      string ghPath = Path.Combine(rep, GITHUB_FOLDER);
      if (!Directory.Exists(ghPath))
      {
        throw new WorkflowValidationException($"Directory {GITHUB_FOLDER} does not exist. Nothing to check.");
      }

      string wfPath = Path.Combine(ghPath, WORKFLOWS_FOLDER);
      if (!Directory.Exists(wfPath))
      {
        throw new WorkflowValidationException(
          $"Directory {Path.Combine(GITHUB_FOLDER, WORKFLOWS_FOLDER)} does not exist. Nothing to check.");
      }

      return wfPath;
    }
  }
}