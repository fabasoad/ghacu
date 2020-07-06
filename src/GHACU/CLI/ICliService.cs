namespace GHACU.CLI
{
  public interface ICliService
  {
    void Run(string repository, bool shouldUpgrade);
  }
}