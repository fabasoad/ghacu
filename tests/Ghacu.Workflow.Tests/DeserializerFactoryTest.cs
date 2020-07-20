using System.Collections.Generic;
using System.IO;
using Xunit;
using YamlDotNet.Serialization;

namespace Ghacu.Workflow.Tests
{
  public class DeserializerFactoryTest
  {
    [Fact]
    public void Get_Successfully()
    {
      var factory = new DeserializerFactory();
      IDeserializer deserializer = factory.Get();
      using var reader = new StringReader(@"
testList:
  - item1
  - item2
");
      var actual = deserializer.Deserialize<StubForDeserializer>(reader);
      Assert.NotNull(actual.TestList);
      Assert.Equal(2, actual.TestList.Count);
      Assert.Equal("item1", actual.TestList[0]);
      Assert.Equal("item2", actual.TestList[1]);
    }

    private class StubForDeserializer
    {
      public List<string> TestList { get; set; }
    }
  }
}