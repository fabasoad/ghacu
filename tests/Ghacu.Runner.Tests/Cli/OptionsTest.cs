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
      Assert.Equal("Ghacu.GitHub token to work with actions repositories.", option.HelpText);
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
      Assert.Equal(LogLevel.Error, option.Default);
    }

    [Fact]
    public void NoCache_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("UseCache")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("cache", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Enable cache.", option.HelpText);
      Assert.Equal(BooleanOption.Yes, option.Default);
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

      const BooleanOption expectedUseCache = BooleanOption.Yes;
      options.UseCache = expectedUseCache;
      Assert.Equal(expectedUseCache, options.UseCache);

      const string expectedGitHubToken = "SomeToken";
      options.GitHubToken = expectedGitHubToken;
      Assert.Equal(expectedGitHubToken, options.GitHubToken);
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
    public void UseColor_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options).GetProperty("UseColor")?.GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.NotNull(attrs);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.NotNull(option);
      Assert.Equal("color", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Enable colors in console output.", option.HelpText);
      Assert.Equal(BooleanOption.Yes, option.Default);
    }
  }
}