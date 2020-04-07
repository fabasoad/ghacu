namespace GHACU.Workflow.Entities
{
  public sealed class Uses : IRepositoryAware
  {
    public Uses(string fullName) {
      Type = fullName.StartsWith("docker://") ? UsesType.DOCKER : UsesType.GITHUB;
      var splitted = Type switch
      {
        UsesType.DOCKER => fullName.Substring(9).Split(':'),
        _ => fullName.Split('@')
      };
      FullName = splitted[0];
      Version = splitted[1];
    }

    public string FullName { get; private set; }

    public string Owner { get => FullName.Split('/')[0]; }

    public string Name { get => FullName.Split('/')[1]; }

    public string Version { get; private set; }

    public UsesType Type { get; private set; }
  }
}