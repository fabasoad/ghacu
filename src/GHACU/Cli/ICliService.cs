namespace Ghacu.Runner.Cli
{
  public interface ICliService
  {
    void Run(string repository, bool shouldUpgrade);
  }
}