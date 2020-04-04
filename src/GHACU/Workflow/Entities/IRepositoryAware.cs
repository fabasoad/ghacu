namespace GHACU.Workflow.Entities
{
  public interface IRepositoryAware  
  {
      string FullName { get; }
      string Name { get; }
      string Owner { get; }
  }
}