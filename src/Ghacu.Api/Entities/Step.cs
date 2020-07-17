namespace Ghacu.Api.Entities
{
  public sealed class Step
  {
    private Action _action;
    public string Uses { get; set; }
    public Action Action => _action ??= new Action(Uses);
  }
}