using GHACU.PubSub.Logging;

namespace GHACU.PubSub
{
  public static class Initializer
  {
    private static LoggingSubscriber _loggingSubscriber;

    public static void InitializeSubscribers(bool logging)
    {
      if (logging)
      {
        _loggingSubscriber = new LoggingSubscriber();
      }
    }
  }
}