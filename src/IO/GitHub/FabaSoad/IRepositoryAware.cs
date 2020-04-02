namespace IO.GitHub.FabaSoad
{
  public interface IRepositoryAware  
  {
      string FullName { get; }
      string Name { get; }
      string Owner { get; }
  }
}