using CommandLine;
using GHACU.CLI;
using Xunit;

namespace GHACU.Tests.CLI
{
  public class OptionsTest
  {
    [Fact]
    public void Repository_ConfiguredCorrectly()
    {
      var attrs = typeof(Options).GetProperty("Repository").GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.Equal("r", option.ShortName);
      Assert.Equal("repository", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Path to the root of a project.", option.HelpText);
    }

    [Fact]
    public void Upgrade_ConfiguredCorrectly()
    {
      var attrs = typeof(Options).GetProperty("Upgrade").GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.Equal("u", option.ShortName);
      Assert.Equal("upgrade", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Upgrade versions to the latest one.", option.HelpText);
    }

    [Fact]
    public void Properties_GetSetWorkedCorrectly()
    {
      var options = new Options();

      var expectedRepository = "SomeRepo";
      options.Repository = expectedRepository;
      Assert.Equal(expectedRepository, options.Repository);

      var expectedUpgrade = false;
      options.Upgrade = expectedUpgrade;
      Assert.Equal(expectedUpgrade, options.Upgrade);
    }
  }
}
