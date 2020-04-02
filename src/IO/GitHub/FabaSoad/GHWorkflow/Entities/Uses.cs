namespace IO.GitHub.FabaSoad.GHWorkflow.Entities
{
  public class Uses
  {
    public Uses(string fullName) {
      var splitted = fullName.Split('@');
      Repository = splitted[0];
      Version = splitted[1];
    }
    public string Repository { get; private set; }
    public string Version { get; private set; }
  }
}