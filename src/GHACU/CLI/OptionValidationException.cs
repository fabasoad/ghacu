using System;

namespace GHACU.CLI
{
  internal class OptionValidationException : Exception
  {
    internal OptionValidationException(string message) : base(message) { }
  }
}