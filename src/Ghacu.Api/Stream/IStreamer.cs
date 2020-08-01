namespace Ghacu.Api.Stream
{
  public interface IStreamer
  {
    void Push<T>(StreamOptions options);
    void PushLine<T>(StreamOptions options);
    void PushEmpty();
  }
}