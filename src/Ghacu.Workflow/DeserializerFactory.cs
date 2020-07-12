using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Ghacu.Workflow
{
  public class DeserializerFactory : IDeserializerFactory
  {
    public IDeserializer Get() => new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .IgnoreUnmatchedProperties()
      .Build();
  }
}