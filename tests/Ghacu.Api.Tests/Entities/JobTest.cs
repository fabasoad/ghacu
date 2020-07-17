using System.Collections.Generic;
using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class JobTest
  {
    [Fact]
    public void Test_GettersSetters()
    {
      var steps = new List<Step> { new Step() };
      var job = new Job { Steps = steps };
      Assert.Equal(steps, job.Steps);
    }
  }
}