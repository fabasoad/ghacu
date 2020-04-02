using System;
using CommandLine;

using IO.GitHub.FabaSoad.CLI;

namespace ghacu
{
  class Program
  {
    public static void Main(string[] args)
    {
      var optionsHandler = new OptionsHandler();
      Parser.Default
        .ParseArguments<Options>(args)
        .WithParsed<Options>(optionsHandler.Handle);
    }
  }
}
