using System;
using Ghacu.Runner.Cache;
using Xunit;

namespace ghacu.Tests.Cache
{
  public class ActionDtoTest
  {
    [Fact]
    public void Properties_GetSetWorkedCorrectly()
    {
      var name = "SomeName";
      var version = "SomeVersion";
      var timestamp = DateTime.Now;
      var dbAction = new ActionDto
      {
        Name = name, Timestamp = timestamp, Version = version
      };
      Assert.Equal(name, dbAction.Name);
      Assert.Equal(version, dbAction.Version);
      Assert.Equal(timestamp, dbAction.Timestamp);
    }
  }
}