using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ghacu.Api.Entities;
using Ghacu.Workflow.Exceptions;
using Telerik.JustMock;
using Xunit;

namespace Ghacu.Workflow.Tests
{
  public class WorkflowServiceTest
  {
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    [InlineData(".")]
    public void GetWorkflows_GitHubFolderDoesNotExist(string path)
    {
      var service = new WorkflowService(null);
      Assert.Throws<WorkflowValidationException>(() => service.GetWorkflows(path));
    }

    [Fact]
    public void GetWorkflows_WorkflowsFolderDoesNotExist()
    {
      string path = Directory.GetCurrentDirectory();
      DirectoryInfo githubFolder = Directory.CreateDirectory(Path.Combine(path, ".github"));
      var service = new WorkflowService(null);
      try
      {
        Assert.Throws<WorkflowValidationException>(() => service.GetWorkflows(path));
      }
      finally
      {
        githubFolder.Delete(true);
      }
    }

    [Fact]
    public void GetWorkflows_BasePathDoesNotExist()
    {
      var service = new WorkflowService(null);
      Assert.Throws<WorkflowValidationException>(() => service.GetWorkflows("not-existing-path"));
    }

    [Fact]
    public void GetWorkflows_Positive()
    {
      const string baseFolderName = "ghacu";
      int ghacuIndex =
        AppContext.BaseDirectory.IndexOf(
          baseFolderName + Path.DirectorySeparatorChar + "tests", StringComparison.Ordinal)
        + baseFolderName.Length;
      string path = AppContext.BaseDirectory.Substring(0, ghacuIndex);
      const string file1 = "test-file1";
      const string file2 = "test-file2";
      var wfi1 = new WorkflowInfo(file1, null);
      var wfi2 = new WorkflowInfo(file2, null);
      var parserMock = Mock.Create<IWorkflowParser>();
      Mock.Arrange(() => parserMock.Parse(
          Arg.Matches<IEnumerable<string>>(files => files.All(
            f => Path.GetExtension(f) == ".yml" || Path.GetExtension(f) == ".yaml"))))
        .Returns(new[] { wfi1, wfi2 });
      var service = new WorkflowService(parserMock);
      IEnumerable<WorkflowInfo> actual = service.GetWorkflows(path);
      Assert.Equal(2, actual.Count());
      Assert.Contains(wfi1, actual);
      Assert.Contains(wfi2, actual);
    }
  }
}