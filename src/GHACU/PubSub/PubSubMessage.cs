namespace GHACU.PubSub
{
  public class PubSubMessage
  {
    public PubSubAction Action { get; set; }
    public PubSubTopic Topic { get; set; }
    public string Message { get; set; }
  }
}