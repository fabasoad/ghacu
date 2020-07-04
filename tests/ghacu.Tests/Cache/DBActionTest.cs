using System;
using GHACU.Cache;
using Xunit;

namespace ghacu.Tests.Cache
{
  public class DBActionTest
  {
    [Fact]
    public void Properties_GetSetWorkedCorrectly()
    {
      var name = "SomeName";
      var version = "SomeVersion";
      var timestamp = DateTime.Now;
      var dbAction = new DBAction
      {
        Name = name, Timestamp = timestamp, Version = version
      };
      Assert.Equal(name, dbAction.Name);
      Assert.Equal(version, dbAction.Version);
      Assert.Equal(timestamp, dbAction.Timestamp);
    }
  }
}