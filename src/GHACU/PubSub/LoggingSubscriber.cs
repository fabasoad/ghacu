using System;
using PubSub;

namespace GHACU.PubSub.Logging
{
  public class LoggingSubscriber
  {
    private readonly Hub _hub;

    public LoggingSubscriber()
    {
      _hub = Hub.Default;
      _hub.Subscribe<PubSubMessage>(LogGitHubTopic);
    }

    private void LogGitHubTopic(PubSubMessage topic)
    {
      Console.WriteLine($"[{topic.Topic}][{topic.Action}] {topic.Message}");
    }
  }
}