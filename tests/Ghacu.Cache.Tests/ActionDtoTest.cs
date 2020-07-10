using System;
using Xunit;

namespace Ghacu.Cache.Tests
{
  public class ActionDtoTest
  {
    [Fact]
    public void Properties_GetSetWorkedCorrectly()
    {
      const string name = "SomeName";
      const string version = "SomeVersion";
      DateTime timestamp = DateTime.Now;
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