using System;
using CommandLine;
using GHACU.CLI;

namespace GHACU
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
