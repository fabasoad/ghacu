using System.Collections.Generic;
using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class ActionWorkflowTest
  {
    [Fact]
    public void Test_GettersSetters()
    {
      const string name = "test-name";
      var jobs = new Dictionary<string, Job>
      {
        { "some-key", new Job() },
      };
      var actionWorkflow = new ActionWorkflow { Name = name, Jobs = jobs };
      Assert.Equal(name, actionWorkflow.Name);
      Assert.Equal(jobs, actionWorkflow.Jobs);
    }
  }
}