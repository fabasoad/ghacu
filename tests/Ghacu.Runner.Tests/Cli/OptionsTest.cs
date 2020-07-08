namespace Ghacu.Runner.Tests.Cli
{
  using CommandLine;
  using Ghacu.Runner.Cli;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public class OptionsTest
  {
    [Fact]
    public void Repository_ConfiguredCorrectly()
    {
      object[] attrs = typeof(Options)
        .GetProperty("Repository")?.GetCustomAttributes(typeof(OptionAttribute), false);
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
    public void LogLevel_ConfiguredCorrectly()
    {
      var attrs = typeof(Options).GetProperty("LogLevel").GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.Equal("l", option.ShortName);
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
      var attrs = typeof(Options).GetProperty("NoCache").GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.Equal("n", option.ShortName);
      Assert.Equal("no-cache", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Turn it on if you do not want to use caching.", option.HelpText);
    }

    [Fact]
    public void GitHubToken_ConfiguredCorrectly()
    {
      var attrs = typeof(Options).GetProperty("GitHubToken").GetCustomAttributes(typeof(OptionAttribute), false);
      Assert.Single(attrs);
      var option = attrs[0] as OptionAttribute;
      Assert.Equal("t", option.ShortName);
      Assert.Equal("token", option.LongName);
      Assert.False(option.Required);
      Assert.Equal("Ghacu.GitHub token to work with actions repositories.", option.HelpText);
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
    }
  }
}