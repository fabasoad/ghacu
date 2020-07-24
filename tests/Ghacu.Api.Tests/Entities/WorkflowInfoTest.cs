using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ghacu.Api.Entities;
using Xunit;

namespace Ghacu.Api.Tests.Entities
{
  public class WorkflowInfoTest
  {
    [Fact]
    public void Test_GettersSetters()
    {
      const string file = "test-file";
      var actionWorkflow = new ActionWorkflow();
      var wfi = new WorkflowInfo(file, actionWorkflow);
      Assert.NotNull(wfi.File);
      Assert.Equal(file, wfi.File.FilePath);
      Assert.Same(actionWorkflow, wfi.Workflow);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("      ")]
    [InlineData("non-existing-path")]
    public void Upgrade_InvalidFile(string file) => new WorkflowInfo(file, null).Upgrade();

    [Theory]
    [InlineData(
      new[] { "v1", "v1", "v1", "v1", "v1", "v1", "v1" },
      new[] { "v2", "v1.1", "v1.0.1", "unknown", "latest", "v1.0.1-preview.7", "v1.0.1-preview.6+57" },
      new[] { "v2", "v1.1", "v1.0.1", "v1", "v1", "v1.0.1-preview.7", "v1.0.1-preview.6+57" })]
    [InlineData(
      new[] { "v1", "v1", "v1", "v1", "v1", "v1", "v1" },
      new[] { "v1.0", "v1.0.0", "v1.0.0-alpha.1", "v1.0.0-alpha.1+6574", "v1.0-alpha.1", "v1-alpha.1+6574", "v1" },
      new[] { "v1.0", "v1.0.0", "v1", "v1", "v1", "v1", "v1" })]
    [InlineData(
      new[] { "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1" },
      new[] { "v2", "v1.2", "v1.1.1", "v1.1.1-preview.7", "v1.1.1-preview.6+57", "v1.1-preview.7", "v1-preview.6+57" },
      new[] { "v2", "v1.2", "v1.1.1", "v1.1.1-preview.7", "v1.1.1-preview.6+57", "v1.1", "v1.1" })]
    [InlineData(
      new[] { "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1" },
      new[] { "v1.1.0", "v1.1.1-alpha.1", "v1.1.1-alpha.1+3457", "v1.1", "v1", "v1.0-preview.7", "v1.0" },
      new[] { "v1.1.0", "v1.1.1-alpha.1", "v1.1.1-alpha.1+3457", "v1.1", "v1.1", "v1.1", "v1.1" })]
    [InlineData(
      new[] { "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1" },
      new[] { "v1.0", "v0.2", "v1.1-alpha.1+3457", "v0.0.2", "unknown", "v1.0-preview.7", "latest" },
      new[] { "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1", "v1.1" })]
    [InlineData(
      new[] { "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" },
      new[] { "v2", "v1.2", "v1.1.2", "v1.1.1-preview.7", "v1.1.1-preview.6+57", "v1.1-preview.7", "v1-preview.6+57" },
      new[] { "v2", "v1.2", "v1.1.2", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" })]
    [InlineData(
      new[] { "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" },
      new[] { "v1.1.1", "v1.1.0-alpha.1", "v1.1.0-alpha.1+2362", "v1.1-beta.1", "v1-preview.6", "v1.1.0", "v1.0.2" },
      new[] { "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" })]
    [InlineData(
      new[] { "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" },
      new[] { "v1.2.1", "v2.0.1", "v0.2.2", "v0.2.2-beta.1", "v1.0.2-preview.6", "latest", "unknown" },
      new[] { "v1.2.1", "v2.0.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1", "v1.1.1" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2" },
      new[] { "v1", "v1.1", "v1.1.1", "v1.1.1-alpha.2", "v1.1.1-alpha.2+3743", "latest", "uknown" },
      new[] { "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1", "v1.1.1-alpha.2", "v1.1.1-alpha.2+3743", "v1.1.1-alpha.2", "v1.1.1-alpha.2" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2" },
      new[] { "v1.0", "v1.1.0", "v1.1.1-alpha.1", "v1.1.1-beta.1", "v1.1.1-alpha.1+3743", "v1.0.5", "v1.1.1-alpha.3" },
      new[] { "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-beta.1", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.3" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2", "v1.1.1-alpha.2" },
      new[] { "v2", "v2.0", "v2.0.0", "v1.2", "v1.2.0", "v1.1.2", "v0.2.2-beta.1" },
      new[] { "v2", "v2.0", "v2.0.0", "v1.2", "v1.2.0", "v1.1.2", "v1.1.1-alpha.2" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233" },
      new[] { "v1", "v1.1", "v1.1.1", "v1.1.1-alpha.2", "v1.1.1-alpha.2+3743", "latest", "uknown" },
      new[] { "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3743", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233" },
      new[] { "v1.0", "v1.1.0", "v1.1.1-alpha.1", "v1.1.1-beta.1", "v1.1.1-alpha.1+3743", "v1.0.5", "v1.1.1-alpha.3" },
      new[] { "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-beta.1", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.3" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233", "v1.1.1-alpha.2+3233" },
      new[] { "v2", "v2.0", "v2.0.0", "v1.2", "v1.2.0", "v1.1.2", "v0.2.2-beta.1" },
      new[] { "v2", "v2.0", "v2.0.0", "v1.2", "v1.2.0", "v1.1.2", "v1.1.1-alpha.2+3233" })]
    [InlineData(
      new[] { "v1.1.1-alpha.2+3233", "latest", "latest", "latest", "latest", "latest", "latest" },
      new[] { "v1.1.1-alpha.2+3232", "v0.0.1", "v0.1", "v1", "v0.0.0-alpha.1", "v0.0.0-alpha.1+1", "latest" },
      new[] { "v1.1.1-alpha.2+3233", "v0.0.1", "v0.1", "v1", "v0.0.0-alpha.1", "v0.0.0-alpha.1+1", "latest" })]
    public void Upgrade_FileValid(string[] current, string[] latest, string[] result)
    {
      var actionWorkflow = new ActionWorkflow
      {
        Jobs = new Dictionary<string, Job>
        {
          {
            "github", new Job
            {
              Steps = current.Select((v, i) =>
              {
                var step = new Step { Uses = $"own{i}/repo{i}@{v}" };
                step.Action.LatestVersion = latest[i];
                return step;
              })
            }
          },
          {
            "docker", new Job
            {
              Steps = current.Select((v, i) =>
              {
                var step = new Step { Uses = $"docker://d{i}.com:{v}" };
                step.Action.LatestVersion = latest[i];
                return step;
              })
            }
          }
        }
      };
      var original = @"
name: Test
jobs:
  github:
    steps:
";
      original += string.Join("\n", current.Select(
        (v, i) => $"      uses: own{i}/repo{i}@{v}"));
      original += @"
  docker:
    steps:
";
      original += string.Join("\n", current.Select(
        (v, i) => $"      uses: docker://d{i}.com:{v}"));
      var expected = @"
name: Test
jobs:
  github:
    steps:
";
      expected += string.Join("\n", result.Select(
        (v, i) => $"      uses: own{i}/repo{i}@{v}"));
      expected += @"
  docker:
    steps:
";
      expected += string.Join("\n", result.Select(
        (v, i) => $"      uses: docker://d{i}.com:{v}"));
      const string file = "./test.yml";
      File.WriteAllText(file, original);
      try
      {
        var wfi = new WorkflowInfo(file, actionWorkflow);
        wfi.Upgrade();
        Assert.Equal(expected, File.ReadAllText(file));
      }
      finally
      {
        File.Delete(file);
      }
    }
  }
}