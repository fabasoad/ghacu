namespace Ghacu.Api.Entities
{
  public sealed class WorkflowFile
  {
    public WorkflowFile(string path)
    {
      FilePath = path;
    }

    public string Name => FilePath.Substring(FilePath.IndexOf(".github"));
    public string FilePath { get; }
  }
}