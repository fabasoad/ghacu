using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ghacu.Api.Entities;
using Telerik.JustMock;
using Xunit;
using YamlDotNet.Serialization;

namespace Ghacu.Workflow.Tests
{
  public class WorkflowParserTest : IClassFixture<FilesFixture>
  {
    private readonly FilesFixture _fixture;

    public WorkflowParserTest(FilesFixture fixture)
    {
      _fixture = fixture;
    }
    
    [Fact]
    public void Parse_Positive()
    {
      var actionWorkflow1 = new ActionWorkflow { Name = "workflow-1" };
      var actionWorkflow2 = new ActionWorkflow { Name = "workflow-2" };
      
      var deserializerMock = Mock.Create<IDeserializer>();
      Mock.Arrange(() => deserializerMock.Deserialize<ActionWorkflow>(
        Arg.Matches<StreamReader>(r => Path.GetFileName((r.BaseStream as FileStream).Name) == _fixture.Files[0])))
        .Returns(actionWorkflow1);
      Mock.Arrange(() => deserializerMock.Deserialize<ActionWorkflow>(
        Arg.Matches<StreamReader>(r => Path.GetFileName((r.BaseStream as FileStream).Name) == _fixture.Files[1])))
        .Returns(actionWorkflow2);
      
      var deserializerFactoryMock = Mock.Create<IDeserializerFactory>();
      Mock.Arrange(() => deserializerFactoryMock.Get()).Returns(deserializerMock);
      
      var parser = new WorkflowParser(deserializerFactoryMock);
      IEnumerable<WorkflowInfo> result = parser.Parse(_fixture.Files);
      Assert.Equal(2, result.Count());
      WorkflowInfo workflowInfo = result.First();
      Assert.Equal(_fixture.Files[0], workflowInfo.File.FilePath);
      Assert.Equal(actionWorkflow1, workflowInfo.Workflow);
      workflowInfo = result.Last();
      Assert.Equal(_fixture.Files[1], workflowInfo.File.FilePath);
      Assert.Equal(actionWorkflow2, workflowInfo.Workflow);
    }
  }
  
  public class FilesFixture : IDisposable
  {
    public FilesFixture()
    {
      var files = new[] { "test1.temp", "test2.temp" };
      foreach (string file in files)
      {
        File.Create(file).Close();
      }

      Files = files;
    }

    internal string[] Files { get; }

    public void Dispose()
    {
      foreach (string file in Files)
      {
        File.Delete(file);
      }
    }
  }
}