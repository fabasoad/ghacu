using CommandLine;
using Ghacu.Runner.Cli;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Ghacu.Runner.Tests.Cli
{
  public class OptionsTest
  {
    [Fact]
    public void GitHubToken_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("GitHubToken")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("token", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("GitHub token to work with actions repositories.", option.HelpText);
    }

    [Fact]
    public void LogLevel_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("LogLevel")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("log-level", option.LongName);
      Assert.False(option.Required);
      Assert.Equal(
        "Set log level. Possible values: Trace, Debug, Information, Warning, Error, Critical, None.",
        option.HelpText);
      Assert.Equal(LogLevel.Information, option.Default);
    }

    [Fact]
    public void NoCache_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("NoCache")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("no-cache", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Disable cache.", option.HelpText);
      Assert.Equal(false, option.Default);
    }

    [Fact]
    public void NoColors_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("NoColors")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("no-colors", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Disable colors.", option.HelpText);
      Assert.Equal(false, option.Default);
    }

    [Fact]
    public void Properties_GetSetWorkedCorrectly()
    {
      var options = new Options();

      const string expectedRepository = "SomeRepo";
      options.Repository = expectedRepository;
      Assert.Equal(expectedRepository, options.Repository);

      const bool expectedUpgrade = false;
      options.Upgrade = expectedUpgrade;
      Assert.Equal(expectedUpgrade, options.Upgrade);

      const LogLevel expectedLogLevel = LogLevel.None;
      options.LogLevel = expectedLogLevel;
      Assert.Equal(expectedLogLevel, options.LogLevel);

      const bool expectedNoCache = true;
      options.NoCache = expectedNoCache;
      Assert.Equal(expectedNoCache, options.NoCache);

      const string expectedGitHubToken = "SomeToken";
      options.GitHubToken = expectedGitHubToken;
      Assert.Equal(expectedGitHubToken, options.GitHubToken);

      const OutputType outputType = OutputType.Console;
      options.OutputType = outputType;
      Assert.Equal(outputType, options.OutputType);

      const bool expectedNoColors = true;
      options.NoColors = expectedNoColors;
      Assert.Equal(expectedNoColors, options.NoColors);
    }

    [Fact]
    public void Repository_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("Repository")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("repository", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Path to the root of a project.", option.HelpText);
    }

    [Fact]
    public void Upgrade_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("Upgrade")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("upgrade", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Upgrade versions to the latest one.", option.HelpText);
    }

    [Fact]
    public void OutputType_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("OutputType")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("output-type", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Information output type. Possible values: Console, Logger, Silent.", option.HelpText);
      Assert.Equal(OutputType.Console, option.Default);
    }
  }
}