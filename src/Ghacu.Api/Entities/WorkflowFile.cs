using System;

namespace Ghacu.Api.Entities
{
  public sealed class WorkflowFile
  {
    public WorkflowFile(string path)
    {
      FilePath = path;
    }

    public string Name
    {
      get
      {
        int index = FilePath?.IndexOf(".github", StringComparison.Ordinal) ?? -1;
        // ReSharper disable once PossibleNullReferenceException
        return index < 0 ? string.Empty : FilePath.Substring(index);
      }
    }

    public string FilePath { get; }
  }
}