using System;
using Semver;
using YamlDotNet.Serialization;

namespace Ghacu.Api.Entities
{
  public sealed class Step
  {
    private Action _action;
    
    // [YamlMember(Alias = "uses", ApplyNamingConventions = false)]
    public string Uses { get; set; }
    public Action Action => _action ??= new Action(Uses);
  }
}