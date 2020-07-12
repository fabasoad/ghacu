using YamlDotNet.Serialization;

namespace Ghacu.Workflow
{
  public interface IDeserializerFactory
  {
    IDeserializer Get();
  }
}