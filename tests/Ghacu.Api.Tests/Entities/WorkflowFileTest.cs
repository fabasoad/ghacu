using System;
using System.Collections.Generic;
using System.IO;
using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class WorkflowFileTest
  {
    [Theory]
    [MemberData(nameof(DataTestGettersSetters))]
    public void Test_GettersSetters(string path, string name)
    {
      var workflowFile = new WorkflowFile(path);
      Assert.Equal(path, workflowFile.FilePath);
      Assert.Equal(name, workflowFile.Name);
    }

    public static IEnumerable<object[]> DataTestGettersSetters
    {
      get
      {
        const string baseFolderName = "ghacu";
        int ghacuIndex =
          AppContext.BaseDirectory.IndexOf(
            baseFolderName + Path.DirectorySeparatorChar + "tests", StringComparison.Ordinal)
          + baseFolderName.Length;
        string ghacuFolder = AppContext.BaseDirectory.Substring(0, ghacuIndex);
        return new List<object[]>
        {
          new object[] { null, string.Empty },
          new object[] { string.Empty, string.Empty },
          new object[] { "not-existing-path", string.Empty },
          new object[] { AppContext.BaseDirectory, string.Empty },
          new object[] { Path.Combine(ghacuFolder, ".github", "test1.yml"), Path.Combine(".github", "test1.yml") }
        };
      }
    }
  }
}