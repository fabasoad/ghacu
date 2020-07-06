using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Ghacu.Runner.Tests")]
namespace Ghacu.Runner.Cli
{
  internal class OptionValidationException : Exception
  {
    internal OptionValidationException(string message) : base(message) { }
  }
}