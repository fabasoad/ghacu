using System;

namespace Ghacu.Runner.Cli
{
  internal class OptionValidationException : Exception
  {
    internal OptionValidationException(string message) : base(message)
    {
    }
  }
}