namespace GHACU.Workflow.Entities
{
  public interface IStep
  {
    IUses Uses { get; }
    bool IsUpToDate { get; }
    void Upgrade(string fileName);
  }
}