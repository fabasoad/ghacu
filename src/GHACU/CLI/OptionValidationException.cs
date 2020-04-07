using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GHACU.Tests")]
namespace GHACU.CLI
{
  internal class OptionValidationException : Exception
  {
    internal OptionValidationException(string message) : base(message) { }
  }
}